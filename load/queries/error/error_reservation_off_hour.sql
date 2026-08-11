UPDATE dbo.Reservations
SET ReservationDate = DATEADD(MINUTE, 17, ReservationDate)
WHERE ReservationId = (SELECT TOP (1) ReservationId FROM dbo.Reservations ORDER BY ReservationId);
