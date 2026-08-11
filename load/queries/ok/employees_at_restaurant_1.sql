SELECT EmployeeId, FirstName, LastName, Position, RestaurantName
FROM dbo.vw_EmployeeDetails
WHERE RestaurantId = 1
ORDER BY EmployeeId;
