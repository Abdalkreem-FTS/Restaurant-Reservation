SELECT CustomerId, FirstName, LastName, Email
FROM dbo.Customers
WHERE LastName LIKE 'W%'
ORDER BY LastName, FirstName;
