SELECT COUNT(o.OrderId) AS OrderCount, ISNULL(SUM(o.TotalAmount), 0) AS TotalTaken,
       ISNULL(AVG(o.TotalAmount), 0) AS AverageOrder
FROM dbo.Orders AS o
WHERE o.EmployeeId = 3;
