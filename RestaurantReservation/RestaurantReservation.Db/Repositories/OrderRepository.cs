using Microsoft.Extensions.Logging;
using RestaurantReservation.Db.Abstractions;
using RestaurantReservation.Db.Entities;
using RestaurantReservation.Db.Pagination;

namespace RestaurantReservation.Db.Repositories;

public class OrderRepository(RestaurantReservationDbContext context, ILogger<OrderRepository> logger) : Repository<Order>(context, logger), IOrderRepository
{
    /// <summary>
    /// Split into one query per collection level. A single query would left-join the order items onto
    /// their order, repeating every order column once per item; splitting keeps each row narrow at the
    /// cost of an extra round trip.
    /// </summary>
    public async Task<PagedResult<Order>> ListOrdersAndMenuItemsAsync(int reservationId, PageRequest page, CancellationToken cancellationToken = default)
    {
        return await Context.Orders
            .Where(o => o.ReservationId == reservationId)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.MenuItem)
            .AsNoTracking()
            .AsSplitQuery()
            .OrderBy(o => o.OrderId)
            .GetPageAsync(page, cancellationToken);
    }

    public async Task<PagedResult<MenuItem>> ListOrderedMenuItemsAsync(int reservationId, PageRequest page, CancellationToken cancellationToken = default)
    {
        return await Context.OrderItems
            .Where(oi => oi.Order.ReservationId == reservationId)
            .Select(oi => oi.MenuItem)
            .Distinct()
            .AsNoTracking()
            .OrderBy(m => m.ItemId)
            .GetPageAsync(page, cancellationToken);
    }
}
