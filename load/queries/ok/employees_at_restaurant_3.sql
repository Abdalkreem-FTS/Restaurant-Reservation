SELECT EmployeeId, FirstName, LastName, Position, RestaurantName
FROM dbo.vw_EmployeeDetails
WHERE RestaurantId = 3
ORDER BY EmployeeId;
