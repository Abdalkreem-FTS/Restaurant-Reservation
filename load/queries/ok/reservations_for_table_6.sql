SELECT ReservationId, ReservationDate, PartySize, CustomerId
FROM dbo.Reservations
WHERE TableId = 6
ORDER BY ReservationDate;
