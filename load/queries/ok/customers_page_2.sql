SELECT CustomerId, FirstName, LastName, Email, PhoneNumber
FROM dbo.Customers
ORDER BY CustomerId
OFFSET 2 ROWS FETCH NEXT 2 ROWS ONLY;
