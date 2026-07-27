using Microsoft.EntityFrameworkCore.Storage;
using RestaurantReservation.Db.Abstractions;
using RestaurantReservation.Db.Results;

namespace RestaurantReservation.Db;

public class UnitOfWork(RestaurantReservationDbContext context) : IUnitOfWork
{
    private readonly RestaurantReservationDbContext _context = context ?? throw new ArgumentNullException(nameof(context));

    public async Task<Result<IDbContextTransaction>> BeginTransactionAsync(CancellationToken cancellationToken = default)
        =>  Result<IDbContextTransaction>.From(await _context.Database.BeginTransactionAsync(cancellationToken));

    public async Task<Result<int>> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.SaveChangesAsync(cancellationToken);
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
