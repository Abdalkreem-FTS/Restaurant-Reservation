SELECT c.CustomerId, c.FirstName, c.LastName
FROM dbo.Customers AS c
WHERE NOT EXISTS (SELECT 1 FROM dbo.Reservations AS r WHERE r.CustomerId = c.CustomerId)
ORDER BY c.CustomerId;
