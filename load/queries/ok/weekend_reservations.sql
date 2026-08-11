SELECT ReservationId, ReservationDate, PartySize
FROM dbo.Reservations
WHERE DATENAME(WEEKDAY, ReservationDate) IN ('Saturday', 'Sunday')
ORDER BY ReservationDate;
