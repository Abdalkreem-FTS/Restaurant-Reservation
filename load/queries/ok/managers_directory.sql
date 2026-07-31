SELECT e.FirstName, e.LastName, r.Name AS RestaurantName, r.PhoneNumber
FROM dbo.Employees AS e
INNER JOIN dbo.Restaurants AS r ON r.RestaurantId = e.RestaurantId
WHERE e.Position = 0
ORDER BY r.Name;
