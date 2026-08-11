using Microsoft.Extensions.Logging;
using RestaurantReservation.Db.Abstractions;
using RestaurantReservation.Db.Models;
using RestaurantReservation.Db.Results;

namespace RestaurantReservation.Db;

public sealed class SqlScriptExecutor(RestaurantReservationDbContext context, ILogger<SqlScriptExecutor> logger) : ISqlScriptExecutor
{
    public async Task<Result<SqlScriptOutcome>> ExecuteAsync(
        string sql,
        string scriptName,
        TimeSpan? commandTimeout = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(sql))
        {
            return DatabaseError.ScriptEmpty;
        }

        if (commandTimeout is { } timeout)
        {
            context.Database.SetCommandTimeout(timeout);
        }

        // Tags everything the pipeline logs for this run with the script's name; the timing,
        // levels and failure details themselves stay the pipeline's own.
        using var scope = logger.BeginScope("Script {ScriptName}", scriptName);

        try
        {
            var rowsAffected = await context.Database.ExecuteSqlRawAsync(sql, cancellationToken);

            return new SqlScriptOutcome(Math.Max(rowsAffected, 0));
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            return DbExceptionTranslator.Translate(exception);
        }
    }
}
