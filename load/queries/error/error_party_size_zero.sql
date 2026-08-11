UPDATE dbo.Reservations
SET PartySize = 0
WHERE ReservationId = (SELECT TOP (1) ReservationId FROM dbo.Reservations ORDER BY ReservationId);
