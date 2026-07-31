SELECT TOP (5) m.Name, m.Price, r.Name AS RestaurantName
FROM dbo.MenuItems AS m
INNER JOIN dbo.Restaurants AS r ON r.RestaurantId = m.RestaurantId
ORDER BY m.Price DESC;
