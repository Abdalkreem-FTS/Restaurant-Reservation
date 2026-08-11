SELECT OrderId, OrderDate, TotalAmount, ReservationId
FROM dbo.Orders
WHERE EmployeeId = 3
ORDER BY OrderDate DESC;
