using RestaurantReservation.Db.Entities;

namespace RestaurantReservation.Db.Abstractions;

public interface IOrderRepository : IRepository<Order>
{
    /// <summary>
    /// Lists the orders placed on a reservation, each with its order items and their menu items.
    /// </summary>
    Task<IReadOnlyList<Order>> ListOrdersAndMenuItemsAsync(int reservationId, CancellationToken cancellationToken = default);

    /// <summary>Returns the distinct menu items ordered within the given reservation.</summary>
    Task<IReadOnlyList<MenuItem>> ListOrderedMenuItemsAsync(int reservationId, CancellationToken cancellationToken = default);
}