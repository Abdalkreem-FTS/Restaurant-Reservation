SELECT OrderId, OrderDate, TotalAmount, RestaurantId
FROM dbo.Orders
WHERE TotalAmount > 50
ORDER BY TotalAmount DESC;
