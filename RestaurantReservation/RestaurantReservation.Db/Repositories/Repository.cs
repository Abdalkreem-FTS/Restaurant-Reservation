using RestaurantReservation.Db.Abstractions;

namespace RestaurantReservation.Db.Repositories;

/// <summary>
/// Base repository providing the shared asynchronous CRUD implementation over a
/// <see cref="RestaurantReservationDbContext" />. Entity-specific repositories derive from this and
/// add their specialized query methods.
/// </summary>
public abstract class Repository<TEntity>(RestaurantReservationDbContext context) : IRepository<TEntity> where TEntity : class
{
    protected readonly RestaurantReservationDbContext Context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly DbSet<TEntity> _dbSet = context.Set<TEntity>();

    public virtual async Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync([id], cancellationToken);
    }

    public virtual async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking().ToListAsync(cancellationToken);
    }

    public virtual async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        
        await _dbSet.AddAsync(entity, cancellationToken);
        await Context.SaveChangesAsync(cancellationToken);
        
        return entity;
    }

    public virtual async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        
        _dbSet.Update(entity);
        
        await Context.SaveChangesAsync(cancellationToken);
    }

    public virtual async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbSet.FindAsync([id], cancellationToken) ?? throw new KeyNotFoundException($"{typeof(TEntity).Name} with id {id} was not found.");

        _dbSet.Remove(entity);
        
        await Context.SaveChangesAsync(cancellationToken);
    }
}