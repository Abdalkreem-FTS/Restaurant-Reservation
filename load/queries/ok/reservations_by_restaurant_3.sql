SELECT r.ReservationId, r.ReservationDate, r.PartySize, t.Capacity
FROM dbo.Reservations AS r
INNER JOIN dbo.Tables AS t ON t.TableId = r.TableId
WHERE r.RestaurantId = 3
ORDER BY r.ReservationDate;
