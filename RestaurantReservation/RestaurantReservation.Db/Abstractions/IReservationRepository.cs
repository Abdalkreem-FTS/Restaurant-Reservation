using RestaurantReservation.Db.Entities;
using RestaurantReservation.Db.Entities.Views;
using RestaurantReservation.Db.Pagination;

namespace RestaurantReservation.Db.Abstractions;

public interface IReservationRepository : IRepository<Reservation>
{
    /// <summary>
    /// Returns a page of reservations made by the given customer, including restaurant and table,
    /// most recent first.
    /// </summary>
    Task<PagedResult<Reservation>> GetReservationsByCustomerAsync(int customerId, PageRequest page, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns a page of reservations with their customer and restaurant information from the database view.
    /// </summary>
    Task<PagedResult<ReservationDetail>> GetReservationDetailsAsync(PageRequest page, CancellationToken cancellationToken = default);
}
