SELECT OrderId, OrderDate, TotalAmount, RestaurantId
FROM dbo.Orders
WHERE TotalAmount > 10
ORDER BY TotalAmount DESC;
