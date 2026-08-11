namespace RestaurantReservation.API.Contracts.Reservations;

public sealed record CreateReservationRequest(
    int CustomerId,
    int TableId,
    DateTimeOffset ReservationDate,
    int PartySize);

public sealed record UpdateReservationRequest(
    int CustomerId,
    int TableId,
    DateTimeOffset ReservationDate,
    int PartySize);
