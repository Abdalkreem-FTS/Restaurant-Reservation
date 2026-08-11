using RestaurantReservation.Db.Models;
using RestaurantReservation.Db.Results;

namespace RestaurantReservation.Db.Abstractions;

/// <summary>
/// Runs an ad-hoc SQL batch through the same command pipeline the repositories use, so its
/// execution shows up in the command logging like any other database work.
/// </summary>
public interface ISqlScriptExecutor
{
    /// <param name="sql">The SQL batch to execute. Batch separators such as <c>GO</c> are not supported.</param>
    /// <param name="scriptName">Name the run is logged under, typically the file the SQL came from.</param>
    /// <param name="commandTimeout">How long the command may run before the database call is abandoned.</param>
    /// <param name="cancellationToken">Token to observe.</param>
    Task<Result<SqlScriptOutcome>> ExecuteAsync(
        string sql,
        string scriptName,
        TimeSpan? commandTimeout = null,
        CancellationToken cancellationToken = default);
}
