WAITFOR DELAY '00:00:00.750';
SELECT m.ItemId, m.Name, m.Description, m.Price
FROM dbo.MenuItems AS m
WHERE m.Name LIKE '%a%' OR m.Description LIKE '%a%'
ORDER BY m.Price DESC;
