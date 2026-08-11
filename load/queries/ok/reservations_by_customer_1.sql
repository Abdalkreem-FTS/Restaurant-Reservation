SELECT ReservationId, ReservationDate, PartySize, RestaurantId, TableId
FROM dbo.Reservations
WHERE CustomerId = 1
ORDER BY ReservationDate DESC;
