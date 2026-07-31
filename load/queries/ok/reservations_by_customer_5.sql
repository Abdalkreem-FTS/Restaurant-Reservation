SELECT ReservationId, ReservationDate, PartySize, RestaurantId, TableId
FROM dbo.Reservations
WHERE CustomerId = 5
ORDER BY ReservationDate DESC;
