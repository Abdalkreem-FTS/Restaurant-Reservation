SELECT ItemId, Name, Description, Price
FROM dbo.MenuItems
WHERE RestaurantId = 3
ORDER BY Price DESC;
