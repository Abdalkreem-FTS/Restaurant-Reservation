SELECT OrderId, OrderDate, TotalAmount, RestaurantId
FROM dbo.Orders
WHERE TotalAmount > 100
ORDER BY TotalAmount DESC;
