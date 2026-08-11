SELECT m.ItemId, m.Name, m.Price, r.Name AS RestaurantName
FROM dbo.MenuItems AS m
INNER JOIN dbo.Restaurants AS r ON r.RestaurantId = m.RestaurantId
WHERE m.Price < 25
ORDER BY m.Price;
