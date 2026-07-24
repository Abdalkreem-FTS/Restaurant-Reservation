using RestaurantReservation.Db.Abstractions;
using RestaurantReservation.Db.Entities;

namespace RestaurantReservation.Db.Repositories;

public class OrderRepository(RestaurantReservationDbContext context) : Repository<Order>(context), IOrderRepository
{
    public async Task<IReadOnlyList<Order>> ListOrdersAndMenuItemsAsync(int reservationId, CancellationToken cancellationToken = default)
    {
        return await Context.Orders
            .Where(o => o.ReservationId == reservationId)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.MenuItem)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<MenuItem>> ListOrderedMenuItemsAsync(int reservationId, CancellationToken cancellationToken = default)
    {
        return await Context.OrderItems
            .Where(oi => oi.Order.ReservationId == reservationId)
            .Select(oi => oi.MenuItem)
            .Distinct()
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}