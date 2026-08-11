SELECT ReservationId, ReservationDate, PartySize, CustomerId
FROM dbo.Reservations
WHERE TableId = 2
ORDER BY ReservationDate;
