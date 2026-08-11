SELECT ReservationId, ReservationDate, PartySize, RestaurantId, TableId
FROM dbo.Reservations
WHERE CustomerId = 2
ORDER BY ReservationDate DESC;
