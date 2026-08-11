SELECT ReservationId, ReservationDate, PartySize, CustomerId
FROM dbo.Reservations
WHERE TableId = 4
ORDER BY ReservationDate;
