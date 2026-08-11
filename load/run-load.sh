#!/usr/bin/env bash
set -uo pipefail

here="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
project="$here/../RestaurantReservation/RestaurantReservation.Console"
dll="$project/bin/Release/net10.0/RestaurantReservation.Console.dll"
queries="$here/queries"

DURATION="${DURATION:-3600}"
SQL_TIMEOUT="${SQL_TIMEOUT:-5}"

# Category weights in percent; independent of how many files each directory holds.
W_OK="${W_OK:-62}"
W_WRITE="${W_WRITE:-10}"
W_SLOW="${W_SLOW:-12}"
W_ERROR="${W_ERROR:-13}"
W_TIMEOUT="${W_TIMEOUT:-3}"

if (( W_OK + W_WRITE + W_SLOW + W_ERROR + W_TIMEOUT != 100 )); then
  echo "category weights must sum to 100" >&2
  exit 1
fi

echo "building once so every iteration runs the DLL directly..."
dotnet build "$project" -c Release --nologo -v q || exit 1
echo

declare -A runs=() script_failures=()
total=0 succeeded=0 failed=0 infra=0 consecutive_infra=0

pick_category() {
  local roll=$((RANDOM % 100))
  if   (( roll < W_OK )); then echo ok
  elif (( roll < W_OK + W_WRITE )); then echo write
  elif (( roll < W_OK + W_WRITE + W_SLOW )); then echo slow
  elif (( roll < W_OK + W_WRITE + W_SLOW + W_ERROR )); then echo error
  else echo timeout
  fi
}

summary() {
  echo
  echo "=== load summary: $total run(s) in ${SECONDS}s ==="
  printf '  %-8s  %5s  %s\n' category runs 'failed (exit 5)'
  for category in ok write slow error timeout; do
    printf '  %-8s  %5d  %d\n' "$category" "${runs[$category]:-0}" "${script_failures[$category]:-0}"
  done
  echo "  succeeded: $succeeded, failed: $failed, infrastructure problems: $infra"
}

trap 'summary; exit 0' INT TERM

echo "running for ${DURATION}s against $queries"
echo

while (( SECONDS < DURATION )); do
  category=$(pick_category)
  file=$(find "$queries/$category" -name '*.sql' | shuf -n 1)
  name="$category/$(basename "$file")"

  started=$(date +%s%3N)
  dotnet "$dll" --sql-file "$file" --sql-timeout "$SQL_TIMEOUT" > /dev/null 2>&1
  code=$?
  elapsed=$(( $(date +%s%3N) - started ))

  total=$((total + 1))
  runs[$category]=$(( ${runs[$category]:-0} + 1 ))

  case $code in
    0) succeeded=$((succeeded + 1)); consecutive_infra=0 ;;
    5) failed=$((failed + 1)); script_failures[$category]=$(( ${script_failures[$category]:-0} + 1 )); consecutive_infra=0 ;;
    *) infra=$((infra + 1)); consecutive_infra=$((consecutive_infra + 1)) ;;
  esac

  printf '[%s] #%-4d exit %d  %5d ms  %s\n' "$(date +%H:%M:%S)" "$total" "$code" "$elapsed" "$name"

  if (( consecutive_infra >= 5 )); then
    echo "five runs in a row could not reach the database; giving up" >&2
    summary
    exit 2
  fi

  sleep "$((RANDOM % 2)).$((RANDOM % 10))"
done

summary
