using RestaurantReservation.Db.Entities;
using RestaurantReservation.Db.Pagination;

namespace RestaurantReservation.Db.Abstractions;

public interface IOrderRepository : IRepository<Order>
{
    /// <summary>
    /// Lists a page of orders placed on a reservation, each with its order items and their menu items.
    /// </summary>
    Task<PagedResult<Order>> ListOrdersAndMenuItemsAsync(int reservationId, PageRequest page, CancellationToken cancellationToken = default);

    /// <summary>Returns a page of the distinct menu items ordered within the given reservation.</summary>
    Task<PagedResult<MenuItem>> ListOrderedMenuItemsAsync(int reservationId, PageRequest page, CancellationToken cancellationToken = default);
}
