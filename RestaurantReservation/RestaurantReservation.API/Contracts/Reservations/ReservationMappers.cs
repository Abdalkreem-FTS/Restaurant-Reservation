using RestaurantReservation.API.Contracts.Common;

namespace RestaurantReservation.API.Contracts.Reservations;

public static class ReservationMappers
{
    public static ReservationResponse ToResponse(this Reservation reservation) =>
        new(
            reservation.ReservationId,
            reservation.CustomerId,
            reservation.RestaurantId,
            reservation.TableId,
            reservation.ReservationDate,
            reservation.PartySize);

    public static PagedResponse<ReservationResponse> ToResponse(this PagedResult<Reservation> page) =>
        page.ToPagedResponse(ToResponse);
}
