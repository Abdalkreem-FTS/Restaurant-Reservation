SELECT ItemId, Name, Description, Price
FROM dbo.MenuItems
WHERE RestaurantId = 1
ORDER BY Price DESC;
