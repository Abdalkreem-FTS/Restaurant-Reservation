SELECT c.CustomerId, c.FirstName, c.LastName, SUM(o.TotalAmount) AS TotalSpend
FROM dbo.Customers AS c
INNER JOIN dbo.Reservations AS r ON r.CustomerId = c.CustomerId
INNER JOIN dbo.Orders AS o ON o.ReservationId = r.ReservationId
GROUP BY c.CustomerId, c.FirstName, c.LastName
ORDER BY TotalSpend DESC;
