SELECT COUNT(*) AS OrderCount, SUM(TotalAmount) AS Revenue, AVG(TotalAmount) AS AverageOrder,
       MIN(TotalAmount) AS Smallest, MAX(TotalAmount) AS Largest
FROM dbo.Orders
WHERE RestaurantId = 2;
