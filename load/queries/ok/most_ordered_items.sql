SELECT m.Name, SUM(oi.Quantity) AS UnitsOrdered
FROM dbo.OrderItems AS oi
INNER JOIN dbo.MenuItems AS m ON m.ItemId = oi.ItemId
GROUP BY m.Name
ORDER BY UnitsOrdered DESC;
