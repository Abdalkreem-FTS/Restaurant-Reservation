SELECT DISTINCT SUBSTRING(Email, CHARINDEX('@', Email) + 1, 100) AS Domain
FROM dbo.Customers
ORDER BY Domain;
