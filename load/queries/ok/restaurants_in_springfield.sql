SELECT RestaurantId, Name, Address
FROM dbo.Restaurants
WHERE Address LIKE '%Springfield%'
ORDER BY Name;
