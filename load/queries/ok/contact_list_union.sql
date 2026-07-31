SELECT FirstName + ' ' + LastName AS ContactName, PhoneNumber
FROM dbo.Customers
UNION
SELECT Name, PhoneNumber
FROM dbo.Restaurants
ORDER BY ContactName;
