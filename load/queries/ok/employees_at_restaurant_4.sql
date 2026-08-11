SELECT EmployeeId, FirstName, LastName, Position, RestaurantName
FROM dbo.vw_EmployeeDetails
WHERE RestaurantId = 4
ORDER BY EmployeeId;
