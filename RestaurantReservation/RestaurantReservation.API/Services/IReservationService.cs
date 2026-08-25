using System.Security.Claims;
using RestaurantReservation.API.Contracts.Reservations;

namespace RestaurantReservation.API.Services;

public interface IReservationService
{
    Task<Result<Reservation>> GetAsync(ClaimsPrincipal caller, int reservationId, CancellationToken cancellationToken = default);

    Task<Result<PagedResult<Reservation>>> ListForCustomerAsync(
        ClaimsPrincipal caller,
        int customerId,
        PageRequest page,
        CancellationToken cancellationToken = default);

    Task<Result<Reservation>> CreateAsync(ClaimsPrincipal caller, CreateReservationRequest request, CancellationToken cancellationToken = default);

    Task<Result<Reservation>> UpdateAsync(
        ClaimsPrincipal caller,
        int reservationId,
        UpdateReservationRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<Deleted>> DeleteAsync(ClaimsPrincipal caller, int reservationId, CancellationToken cancellationToken = default);
}
