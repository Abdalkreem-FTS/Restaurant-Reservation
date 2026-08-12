using System.Globalization;
using RestaurantReservation.API.Contracts.Reservations;

namespace RestaurantReservation.API.Grpc;

public static class ReservationGrpcMappers
{
    public static CreateReservationRequest ToRequest(this CreateReservationCommand command) =>
        new(command.CustomerId, command.TableId, ParseDate(command.ReservationDate), command.PartySize);

    public static UpdateReservationRequest ToRequest(this UpdateReservationCommand command) =>
        new(command.CustomerId, command.TableId, ParseDate(command.ReservationDate), command.PartySize);

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
        ReservationDate = reservation.ReservationDate.ToString("O", CultureInfo.InvariantCulture),
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

    private static DateTimeOffset ParseDate(string value) =>
        DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var parsed)
            ? parsed
            : throw Error.Failure(
                "Reservations.ReservationDateNotParsed",
                $"'{value}' is not a date and time with an offset, for example 2028-03-03T19:00:00+05:30.").ToRpcException();
}
