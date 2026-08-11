SELECT ItemId, Name, Description, Price
FROM dbo.MenuItems
WHERE RestaurantId = 5
ORDER BY Price DESC;
