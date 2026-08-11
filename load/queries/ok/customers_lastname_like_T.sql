SELECT CustomerId, FirstName, LastName, Email
FROM dbo.Customers
WHERE LastName LIKE 'T%'
ORDER BY LastName, FirstName;
