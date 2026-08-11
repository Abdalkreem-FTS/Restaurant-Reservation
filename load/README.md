# Load run

Drives the console app with one random SQL file per process for a fixed duration, so the
Elasticsearch logs fill with the mixed traffic of a plausible production service: mostly quick
reads, some writes, and a deliberate share of slow, failing and timed-out commands.

## Usage

```bash
docker compose up -d
dotnet ef database update --project RestaurantReservation/RestaurantReservation.Db

bash load/run-load.sh
```

One file can be run by hand as well:

```bash
dotnet run --project RestaurantReservation/RestaurantReservation.Console -- \
  --sql-file load/queries/error/error_duplicate_email_john.sql --sql-timeout 5
```

## Categories

| Directory  | Files | Weight | What the log shows                                          |
|------------|-------|--------|-------------------------------------------------------------|
| `ok/`      | 144   | 62%    | Debug command logs with EF's own timing                     |
| `write/`   | 20    | 10%    | Same, via self-cleaning writes that leave the data unchanged |
| `slow/`    | 16    | 12%    | Warning `CommandSlow` (over the 500 ms threshold)           |
| `error/`   | 16    | 13%    | `CommandFailed` at Information (constraint violations) or Error (broken SQL) |
| `timeout/` | 4     | 3%     | Warning `Db.Timeout` (8 s batch against a 5 s command timeout) |

Every event a run produces is tagged with a `ScriptName` scope, so Kibana can filter and
aggregate by file. Selection is weighted by category first and uniform within it, so the
traffic mix does not shift when files are added to a directory.

## Knobs

Environment variables, all optional: `DURATION` (seconds, default 3600), `SQL_TIMEOUT`
(seconds, default 5), and the percentage weights `W_OK`, `W_WRITE`, `W_SLOW`, `W_ERROR`,
`W_TIMEOUT`, which must sum to 100.

Exit codes per run: `0` success, `5` the script failed in the database, `2` the database was
unreachable (five in a row abort the load). Ctrl-C prints the summary early.
