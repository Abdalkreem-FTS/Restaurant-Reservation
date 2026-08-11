SELECT ReservationId, ReservationDate, PartySize, CustomerId
FROM dbo.Reservations
WHERE TableId = 3
ORDER BY ReservationDate;
