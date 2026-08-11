namespace RestaurantReservation.API.Contracts.Reservations;

public sealed record CreateReservationRequest(
    int CustomerId,
    int TableId,
    DateTime ReservationDate,
    int PartySize);

public sealed record UpdateReservationRequest(
    int CustomerId,
    int TableId,
    DateTime ReservationDate,
    int PartySize);
