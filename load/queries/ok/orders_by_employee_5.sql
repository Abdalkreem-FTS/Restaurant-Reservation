SELECT OrderId, OrderDate, TotalAmount, ReservationId
FROM dbo.Orders
WHERE EmployeeId = 5
ORDER BY OrderDate DESC;
