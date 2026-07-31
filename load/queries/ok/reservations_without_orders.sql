SELECT r.ReservationId, r.ReservationDate, r.PartySize
FROM dbo.Reservations AS r
WHERE NOT EXISTS (SELECT 1 FROM dbo.Orders AS o WHERE o.ReservationId = r.ReservationId)
ORDER BY r.ReservationId;
