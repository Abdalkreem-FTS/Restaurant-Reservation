UPDATE dbo.Reservations
SET PartySize = TableCapacity + 10
WHERE ReservationId = (SELECT TOP (1) ReservationId FROM dbo.Reservations ORDER BY ReservationId);
