SELECT ReservationId, ReservationDate, PartySize, CustomerId
FROM dbo.Reservations
WHERE TableId = 1
ORDER BY ReservationDate;
