SELECT m.ItemId, m.Name, m.Price
FROM dbo.MenuItems AS m
WHERE NOT EXISTS (SELECT 1 FROM dbo.OrderItems AS oi WHERE oi.ItemId = m.ItemId)
ORDER BY m.ItemId;
