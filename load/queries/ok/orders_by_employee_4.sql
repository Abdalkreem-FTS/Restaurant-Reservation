SELECT OrderId, OrderDate, TotalAmount, ReservationId
FROM dbo.Orders
WHERE EmployeeId = 4
ORDER BY OrderDate DESC;
