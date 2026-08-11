SELECT EmployeeId, FirstName, LastName, Position, RestaurantName
FROM dbo.vw_EmployeeDetails
WHERE RestaurantId = 2
ORDER BY EmployeeId;
