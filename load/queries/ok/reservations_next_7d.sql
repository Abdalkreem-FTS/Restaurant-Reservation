SELECT ReservationId, ReservationDate, PartySize, RestaurantId
FROM dbo.Reservations
WHERE ReservationDate BETWEEN SYSDATETIME() AND DATEADD(DAY, 7, SYSDATETIME())
ORDER BY ReservationDate;
