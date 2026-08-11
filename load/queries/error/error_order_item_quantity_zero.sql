UPDATE dbo.OrderItems
SET Quantity = 0
WHERE OrderItemId = (SELECT TOP (1) OrderItemId FROM dbo.OrderItems ORDER BY OrderItemId);
