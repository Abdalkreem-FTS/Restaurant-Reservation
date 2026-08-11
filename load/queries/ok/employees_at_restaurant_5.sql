SELECT EmployeeId, FirstName, LastName, Position, RestaurantName
FROM dbo.vw_EmployeeDetails
WHERE RestaurantId = 5
ORDER BY EmployeeId;
