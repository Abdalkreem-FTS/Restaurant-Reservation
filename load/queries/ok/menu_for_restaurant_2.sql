SELECT ItemId, Name, Description, Price
FROM dbo.MenuItems
WHERE RestaurantId = 2
ORDER BY Price DESC;
