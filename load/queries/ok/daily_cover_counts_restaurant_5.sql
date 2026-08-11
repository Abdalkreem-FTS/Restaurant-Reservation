SELECT CAST(ReservationDate AS DATE) AS Day, COUNT(*) AS Bookings, SUM(PartySize) AS Covers
FROM dbo.Reservations
WHERE RestaurantId = 5
GROUP BY CAST(ReservationDate AS DATE)
ORDER BY Day;
