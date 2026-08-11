SELECT oi.OrderItemId, oi.OrderId, m.Name, oi.Quantity, oi.UnitPrice,
       oi.Quantity * oi.UnitPrice AS LineTotal
FROM dbo.OrderItems AS oi
INNER JOIN dbo.MenuItems AS m ON m.ItemId = oi.ItemId
WHERE oi.RestaurantId = 3
ORDER BY oi.OrderId, oi.OrderItemId;
