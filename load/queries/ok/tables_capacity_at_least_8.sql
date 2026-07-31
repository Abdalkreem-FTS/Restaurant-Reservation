SELECT t.TableId, t.Capacity, r.Name AS RestaurantName
FROM dbo.Tables AS t
INNER JOIN dbo.Restaurants AS r ON r.RestaurantId = t.RestaurantId
WHERE t.Capacity >= 8
ORDER BY t.Capacity DESC, t.TableId;
