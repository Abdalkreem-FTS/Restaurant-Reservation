SELECT EmployeeId, FirstName, LastName, RestaurantId
FROM dbo.Employees
WHERE Position = 0
ORDER BY LastName;
