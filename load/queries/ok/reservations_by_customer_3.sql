SELECT ReservationId, ReservationDate, PartySize, RestaurantId, TableId
FROM dbo.Reservations
WHERE CustomerId = 3
ORDER BY ReservationDate DESC;
