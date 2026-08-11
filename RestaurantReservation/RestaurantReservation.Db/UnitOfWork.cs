using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using RestaurantReservation.Db.Abstractions;
using RestaurantReservation.Db.Logging;
using RestaurantReservation.Db.Results;

namespace RestaurantReservation.Db;

public class UnitOfWork(RestaurantReservationDbContext context, ILogger<UnitOfWork> logger) : IUnitOfWork
{
    private readonly RestaurantReservationDbContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILogger<UnitOfWork> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task<Result<IDbContextTransaction>> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        return Result<IDbContextTransaction>.From(transaction);
    }

    public async Task<Result<int>> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var rowsAffected = await _context.SaveChangesAsync(cancellationToken);

            UnitOfWorkLog.SaveSucceeded(_logger, rowsAffected);

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

            UnitOfWorkLog.SaveFailed(
                _logger,
                level,
                failure.Error.Code,
                failure.Error.Type,
                failure.SqlErrorNumber,
                failure.ConstraintName,
                level == LogLevel.Error ? exception : null);

            return failure.Error;
        }
    }
}
