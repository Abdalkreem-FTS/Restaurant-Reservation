SELECT CustomerId, FirstName, LastName, Email
FROM dbo.Customers
WHERE LastName LIKE 'B%'
ORDER BY LastName, FirstName;
