SELECT OrderId, OrderDate, TotalAmount, ReservationId
FROM dbo.Orders
WHERE EmployeeId = 6
ORDER BY OrderDate DESC;
