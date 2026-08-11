SELECT c.CustomerId, c.FirstName, c.LastName
FROM dbo.Customers AS c
WHERE EXISTS (
    SELECT 1
    FROM dbo.Reservations AS r
    INNER JOIN dbo.Orders AS o ON o.ReservationId = r.ReservationId
    WHERE r.CustomerId = c.CustomerId)
ORDER BY c.CustomerId;
