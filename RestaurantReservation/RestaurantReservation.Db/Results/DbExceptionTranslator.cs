using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Storage;

namespace RestaurantReservation.Db.Results;

/// <summary>
/// Turns the exception a database call failed with into the <see cref="Error" /> that explains it.
/// Every exception maps to something: the ones that describe a broken integrity rule map to that
/// rule's message, and the rest map to a message for their kind of failure.
/// </summary>
public static partial class DbExceptionTranslator
{
    /// <summary>
    /// Prefixes of the schema's own naming convention. Used to pick the constraint name out of a
    /// message rather than matching the surrounding words, because those words are localized and the
    /// constraint name is not.
    /// </summary>
    private static readonly string[] _constraintPrefixes = ["PK_", "AK_", "FK_", "IX_", "CK_"];

    public static Error Translate(Exception exception)
    {
        if (exception is DbUpdateConcurrencyException)
        {
            return DatabaseError.ConcurrencyConflict;
        }

        var sqlException = FindSqlException(exception);

        return sqlException is not null
            ? FromSqlException(sqlException, WasDeleting(exception))
            : FromNonSqlException(exception);
    }

    /// <summary>
    /// The <see cref="SqlException" /> carrying the store's own error number, which EF wraps one or
    /// more levels deep. Walking the chain rather than reading <c>InnerException</c> directly keeps
    /// this working when a retrying execution strategy adds a layer of its own.
    /// </summary>
    private static SqlException? FindSqlException(Exception exception)
    {
        for (var current = exception; current is not null; current = current.InnerException)
        {
            if (current is SqlException sqlException)
            {
                return sqlException;
            }
        }

        return null;
    }

    /// <summary>
    /// Whether the failing save was removing something. A foreign key reports the same error number
    /// whether write pointed at a row that is not there or a deleted left rows pointing back, and
    /// those need opposite messages; the tracked state of the entities EF blames says which it was,
    /// without depending on the wording of the message.
    /// </summary>
    private static bool WasDeleting(Exception exception) =>
        exception is DbUpdateException { Entries.Count: > 0 } updateException
        && updateException.Entries.Any(entry => entry.State == EntityState.Deleted);

    private static Error FromSqlException(SqlException exception, bool wasDeleting) => exception.Number switch
    {
        2601 or 2627 => DatabaseError.DuplicateKey(ConstraintName(exception.Message)),
        547 => FromConstraintConflict(exception.Message, wasDeleting),
        515 => DatabaseError.RequiredValueMissing(FirstQuotedToken(exception.Message)),
        2628 or 8152 => DatabaseError.ValueTooLong,
        220 or 232 or 8115 => DatabaseError.ValueOutOfRange,
        241 or 245 => DatabaseError.InvalidValueFormat,
        544 => DatabaseError.IdentityInsertNotAllowed,
        1205 => DatabaseError.Deadlock,
        1222 => DatabaseError.LockTimeout,
        3960 or 3961 => DatabaseError.SnapshotConflict,
        -2 => DatabaseError.Timeout,
        4060 or 4062 or 4063 or 18452 or 18456 => DatabaseError.LoginFailed,
        10928 or 10929 or 40501 or 49918 or 49919 or 49920 => DatabaseError.Throttled,
        2 or 53 or 64 or 233 or 10053 or 10054 or 10060 or 10061 or 40197 or 40613 => DatabaseError.Unavailable,
        _ => DatabaseError.UnexpectedSqlError(exception.Number),
    };

    /// <summary>
    /// Splits error 547, which SQL Server raises for check constraints and foreign keys alike, by the
    /// prefix of the constraint that rejected the statement.
    /// </summary>
    private static Error FromConstraintConflict(string message, bool wasDeleting)
    {
        var constraint = ConstraintName(message);

        if (constraint is not null && constraint.StartsWith("CK_", StringComparison.OrdinalIgnoreCase))
        {
            return DatabaseError.CheckViolation(constraint);
        }

        return wasDeleting
            ? DatabaseError.StillReferenced(constraint)
            : DatabaseError.MissingRelatedRecord(constraint);
    }

    private static Error FromNonSqlException(Exception exception) => exception switch
    {
        OperationCanceledException => DatabaseError.Timeout,
        RetryLimitExceededException => DatabaseError.Unavailable,
        DbUpdateException => DatabaseError.SaveFailed,
        _ => DatabaseError.Unexpected(exception),
    };

    private static string? ConstraintName(string message) =>
        QuotedToken()
            .Matches(message)
            .Select(match => match.Groups[1].Value)
            .FirstOrDefault(token => _constraintPrefixes.Any(prefix => token.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)));

    private static string? FirstQuotedToken(string message) =>
        QuotedToken().Match(message) is { Success: true } match ? match.Groups[1].Value : null;

    /// <summary>
    /// A quoted identifier. SQL Server quotes constraint names with double quotes in some messages
    /// and single quotes in others, and qualified names such as <c>'dbo.Reservations'</c> are skipped
    /// by the character class, which leaves the bare identifiers this needs.
    /// </summary>
    [GeneratedRegex("""['"]([A-Za-z0-9_]+)['"]""")]
    private static partial Regex QuotedToken();
}
