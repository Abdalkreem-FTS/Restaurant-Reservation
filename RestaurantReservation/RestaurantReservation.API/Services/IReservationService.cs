using RestaurantReservation.API.Contracts.Reservations;

namespace RestaurantReservation.API.Services;

public interface IReservationService
{
    Task<Result<Reservation>> CreateAsync(CreateReservationRequest request, CancellationToken cancellationToken = default);

    Task<Result<Reservation>> UpdateAsync(int reservationId, UpdateReservationRequest request, CancellationToken cancellationToken = default);

    Task<Result<Deleted>> DeleteAsync(int reservationId, CancellationToken cancellationToken = default);
}
