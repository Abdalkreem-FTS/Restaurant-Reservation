SELECT CAST(ReservationDate AS DATE) AS Day, COUNT(*) AS Reservations
FROM dbo.Reservations
GROUP BY CAST(ReservationDate AS DATE)
ORDER BY Day;
