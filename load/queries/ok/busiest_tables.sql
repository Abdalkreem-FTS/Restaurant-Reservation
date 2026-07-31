SELECT t.TableId, t.RestaurantId, t.Capacity, COUNT(r.ReservationId) AS Bookings
FROM dbo.Tables AS t
LEFT JOIN dbo.Reservations AS r ON r.TableId = t.TableId
GROUP BY t.TableId, t.RestaurantId, t.Capacity
ORDER BY Bookings DESC, t.TableId;
