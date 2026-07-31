SELECT ReservationId, ReservationDate, PartySize, RestaurantId, TableId
FROM dbo.Reservations
WHERE CustomerId = 4
ORDER BY ReservationDate DESC;
