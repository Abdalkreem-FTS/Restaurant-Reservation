SELECT r.Name, COUNT(m.ItemId) AS Items, AVG(m.Price) AS AveragePrice
FROM dbo.Restaurants AS r
LEFT JOIN dbo.MenuItems AS m ON m.RestaurantId = r.RestaurantId
GROUP BY r.Name
ORDER BY AveragePrice DESC;
