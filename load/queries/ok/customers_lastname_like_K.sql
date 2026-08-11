SELECT CustomerId, FirstName, LastName, Email
FROM dbo.Customers
WHERE LastName LIKE 'K%'
ORDER BY LastName, FirstName;
