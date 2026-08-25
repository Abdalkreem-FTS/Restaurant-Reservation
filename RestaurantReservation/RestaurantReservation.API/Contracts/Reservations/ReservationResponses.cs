namespace RestaurantReservation.API.Contracts.Reservations;

public sealed record ReservationResponse(
    int ReservationId,
    int CustomerId,
    int RestaurantId,
    int TableId,
    DateTimeOffset ReservationDate,
    int PartySize);
