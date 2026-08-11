SELECT RestaurantId, Name, Address
FROM dbo.Restaurants
WHERE Address LIKE '%Capital City%'
ORDER BY Name;
