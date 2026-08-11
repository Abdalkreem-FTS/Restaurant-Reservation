SELECT CustomerId, FirstName, LastName, Email
FROM dbo.Customers
WHERE LastName LIKE 'S%'
ORDER BY LastName, FirstName;
