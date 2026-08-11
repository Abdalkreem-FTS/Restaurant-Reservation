SELECT COUNT(*) / (SELECT COUNT(*) FROM dbo.Customers WHERE CustomerId < 0) AS Impossible
FROM dbo.Reservations;
