using System.Linq.Expressions;
using RestaurantReservation.Db.Pagination;
using RestaurantReservation.Db.Results;

namespace RestaurantReservation.Db.Abstractions;

public interface IRepository<TEntity> where TEntity : class
{
    Task<Result<TEntity>> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<PagedResult<TEntity>> GetAllAsync(PageRequest page, CancellationToken cancellationToken = default);

    Task<PagedResult<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, PageRequest page, CancellationToken cancellationToken = default);

    Task<Result<TEntity>> AddAsync(TEntity? entity, CancellationToken cancellationToken = default);

    Result<Updated> Update(TEntity? entity);

    Task<Result<Deleted>> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
