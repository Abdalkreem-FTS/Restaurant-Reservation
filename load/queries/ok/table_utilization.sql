SELECT t.TableId, t.Capacity, COUNT(r.ReservationId) AS Bookings,
       ISNULL(AVG(CAST(r.PartySize AS DECIMAL(10, 2))), 0) AS AverageParty
FROM dbo.Tables AS t
LEFT JOIN dbo.Reservations AS r ON r.TableId = t.TableId
GROUP BY t.TableId, t.Capacity
ORDER BY t.TableId;
