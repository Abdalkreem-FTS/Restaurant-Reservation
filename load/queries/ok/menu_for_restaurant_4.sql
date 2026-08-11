SELECT ItemId, Name, Description, Price
FROM dbo.MenuItems
WHERE RestaurantId = 4
ORDER BY Price DESC;
