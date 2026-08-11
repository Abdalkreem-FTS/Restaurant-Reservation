SELECT TOP (10) OrderId, OrderDate, TotalAmount, RestaurantId, EmployeeId
FROM dbo.Orders
ORDER BY TotalAmount DESC;
