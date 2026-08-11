SELECT OrderId, OrderDate, TotalAmount, RestaurantId
FROM dbo.Orders
WHERE TotalAmount > 75
ORDER BY TotalAmount DESC;
