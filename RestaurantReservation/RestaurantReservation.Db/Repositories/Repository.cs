using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using RestaurantReservation.Db.Abstractions;
using RestaurantReservation.Db.Logging;
using RestaurantReservation.Db.Pagination;
using RestaurantReservation.Db.Results;

namespace RestaurantReservation.Db.Repositories;

/// <summary>
/// Base repository providing the shared asynchronous CRUD implementation over a
/// <see cref="RestaurantReservationDbContext" />. Entity-specific repositories derive from this and
/// add their specialized query methods. Writes only stage the change on the context; call
/// <see cref="IUnitOfWork.SaveChangesAsync" /> to commit them.
/// </summary>
public abstract class Repository<TEntity>(RestaurantReservationDbContext context, ILogger logger) : IRepository<TEntity> where TEntity : class
{
    protected readonly RestaurantReservationDbContext Context = context ?? throw new ArgumentNullException(nameof(context));

    /// <summary>
    /// Declared as <see cref="ILogger" /> rather than <see cref="ILogger{TCategoryName}" /> so each
    /// derived repository can pass its own, which puts its events under its own category.
    /// </summary>
    protected readonly ILogger Logger = logger ?? throw new ArgumentNullException(nameof(logger));

    private readonly DbSet<TEntity> _dbSet = context.Set<TEntity>();

    /// <summary>
    /// Name of the single integer primary key property, read from the model so the base class can
    /// order by it without knowing what each entity calls its key. Pagination needs a deterministic
    /// sort, and the key is the only column guaranteed to provide one.
    /// </summary>
    private readonly string _keyName = context.Model
        .FindEntityType(typeof(TEntity))!
        .FindPrimaryKey()!
        .Properties
        .Single()
        .Name;

    public virtual async Task<Result<TEntity>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbSet.FindAsync([id], cancellationToken);

        if (entity is not null)
        {
            return entity;
        }

        RepositoryLog.EntityNotFound(Logger, typeof(TEntity).Name, id);

        return NotFound(id);
    }

    public virtual async Task<PagedResult<TEntity>> GetAllAsync(PageRequest page, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .OrderBy(entity => EF.Property<int>(entity, _keyName))
            .GetPageAsync(page, cancellationToken);
    }

    public virtual async Task<PagedResult<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, PageRequest page, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(predicate)
            .OrderBy(entity => EF.Property<int>(entity, _keyName))
            .GetPageAsync(page, cancellationToken);
    }

    public virtual async Task<Result<TEntity>> AddAsync(TEntity? entity, CancellationToken cancellationToken = default)
    {
        if (entity is null)
        {
            RepositoryLog.NullEntityRejected(Logger, typeof(TEntity).Name, nameof(AddAsync));

            return Error.Validation("Repository.NullEntity", $"{typeof(TEntity).Name} entity must not be null.");
        }

        await _dbSet.AddAsync(entity, cancellationToken);

        return entity;
    }

    public virtual Result<Updated> Update(TEntity? entity)
    {
        if (entity is null)
        {
            RepositoryLog.NullEntityRejected(Logger, typeof(TEntity).Name, nameof(Update));

            return Error.Validation("Repository.NullEntity", $"{typeof(TEntity).Name} entity must not be null.");
        }

        _dbSet.Update(entity);

        return Result.Updated;
    }

    public virtual async Task<Result<Deleted>> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbSet.FindAsync([id], cancellationToken);

        if (entity is null)
        {
            RepositoryLog.EntityNotFound(Logger, typeof(TEntity).Name, id);

            return NotFound(id);
        }

        _dbSet.Remove(entity);

        return Result.Deleted;
    }

    private static Error NotFound(int id) => Error.NotFound("Repository.NotFound", $"{typeof(TEntity).Name} with id {id} was not found.");
}
