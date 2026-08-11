SELECT Position, COUNT(*) AS Headcount
FROM dbo.Employees
GROUP BY Position
ORDER BY Position;
