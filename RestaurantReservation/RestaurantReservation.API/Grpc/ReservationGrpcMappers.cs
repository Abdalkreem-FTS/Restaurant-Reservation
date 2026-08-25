using Google.Protobuf.WellKnownTypes;
using RestaurantReservation.API.Contracts.Reservations;

namespace RestaurantReservation.API.Grpc;

public static class ReservationGrpcMappers
{
    public static CreateReservationRequest ToRequest(this CreateReservationCommand command) =>
        new(command.CustomerId, command.TableId, command.ReservationDate.ToDateTimeOffsetOrDefault(), command.PartySize);

    public static UpdateReservationRequest ToRequest(this UpdateReservationCommand command) =>
        new(command.CustomerId, command.TableId, command.ReservationDate.ToDateTimeOffsetOrDefault(), command.PartySize);

    public static PageRequest ToPageRequest(this ListReservationsQuery query) => new()
    {
        Number = query.HasPage ? query.Page : 1,
        Size = query.HasPageSize ? query.PageSize : PageRequest.DefaultSize
    };

    public static ReservationMessage ToMessage(this Reservation reservation) => new()
    {
        ReservationId = reservation.ReservationId,
        CustomerId = reservation.CustomerId,
        RestaurantId = reservation.RestaurantId,
        TableId = reservation.TableId,
        ReservationDate = Timestamp.FromDateTimeOffset(reservation.ReservationDate),
        PartySize = reservation.PartySize
    };

    public static ReservationPage ToMessage(this PagedResult<Reservation> page)
    {
        var message = new ReservationPage
        {
            PageNumber = page.PageNumber,
            PageSize = page.PageSize,
            TotalCount = page.TotalCount,
            TotalPages = page.TotalPages,
            HasPrevious = page.HasPrevious,
            HasNext = page.HasNext
        };

        message.Items.AddRange(page.Items.Select(ToMessage));

        return message;
    }

    /// <summary>
    /// An unset message field arrives as null. Mapping it to the default date rather than throwing
    /// keeps this layer incapable of failing, so the caller is authorized before anything about the
    /// payload is judged; the validator then reports the date as being in the past.
    /// </summary>
    private static DateTimeOffset ToDateTimeOffsetOrDefault(this Timestamp? timestamp) =>
        timestamp?.ToDateTimeOffset() ?? default;
}
