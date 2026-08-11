SELECT OrderId, OrderDate, TotalAmount, RestaurantId
FROM dbo.Orders
WHERE TotalAmount > 25
ORDER BY TotalAmount DESC;
