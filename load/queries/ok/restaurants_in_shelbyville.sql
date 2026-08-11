SELECT RestaurantId, Name, Address
FROM dbo.Restaurants
WHERE Address LIKE '%Shelbyville%'
ORDER BY Name;
