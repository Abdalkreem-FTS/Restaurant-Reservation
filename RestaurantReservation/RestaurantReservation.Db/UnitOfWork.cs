using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using RestaurantReservation.Db.Abstractions;
using RestaurantReservation.Db.Logging;
using RestaurantReservation.Db.Results;

namespace RestaurantReservation.Db;

public class UnitOfWork(RestaurantReservationDbContext context, ILogger<UnitOfWork> logger) : IUnitOfWork
{
    public async Task<Result<IDbContextTransaction>> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        return Result<IDbContextTransaction>.From(transaction);
    }

    public async Task<Result<int>> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var rowsAffected = await context.SaveChangesAsync(cancellationToken);

            logger.LogDebug(DbEvents.SaveSucceeded, "Saved {RowsAffected} change(s)", rowsAffected);

            return rowsAffected;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            var failure = DbExceptionTranslator.Describe(exception);
            var level = DbLogLevelPolicy.For(failure.Error.Type);

            logger.Log(
                level,
                DbEvents.SaveFailed,
                level == LogLevel.Error ? exception : null,
                "Save failed with {ErrorCode} ({ErrorType}), SQL error {SqlErrorNumber}, constraint {ConstraintName}",
                failure.Error.Code,
                failure.Error.Type,
                failure.SqlErrorNumber,
                failure.ConstraintName);

            return failure.Error;
        }
    }
}
