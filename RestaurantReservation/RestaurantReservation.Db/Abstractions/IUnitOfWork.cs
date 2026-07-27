using Microsoft.EntityFrameworkCore.Storage;
using RestaurantReservation.Db.Results;

namespace RestaurantReservation.Db.Abstractions;

public interface IUnitOfWork
{
    Task<Result<IDbContextTransaction>> BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task<Result<int>> SaveChangesAsync(CancellationToken cancellationToken = default);
}
