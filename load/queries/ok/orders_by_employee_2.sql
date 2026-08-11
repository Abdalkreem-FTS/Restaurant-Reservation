SELECT OrderId, OrderDate, TotalAmount, ReservationId
FROM dbo.Orders
WHERE EmployeeId = 2
ORDER BY OrderDate DESC;
