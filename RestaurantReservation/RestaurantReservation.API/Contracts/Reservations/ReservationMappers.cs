using Riok.Mapperly.Abstractions;

namespace RestaurantReservation.API.Contracts.Reservations;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class ReservationMappers
{
    public static partial ReservationResponse ToResponse(this Reservation reservation);

    public static partial PagedResponse<ReservationResponse> ToResponse(this PagedResult<Reservation> page);
}
