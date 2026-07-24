using RestaurantReservation.Db.Entities;
using RestaurantReservation.Db.Entities.Views;

namespace RestaurantReservation.Db.Abstractions;

public interface IReservationRepository : IRepository<Reservation>
{
    /// <summary>Returns all reservations made by the given customer, including restaurant and table.</summary>
    Task<IReadOnlyList<Reservation>> GetReservationsByCustomerAsync(int customerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns every reservation with its associated customer and restaurant information from the database view.
    /// </summary>
    Task<IReadOnlyList<ReservationDetail>> GetReservationDetailsAsync(CancellationToken cancellationToken = default);
}