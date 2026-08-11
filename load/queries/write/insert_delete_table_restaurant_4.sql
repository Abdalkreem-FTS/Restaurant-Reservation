INSERT INTO dbo.Tables (RestaurantId, Capacity)
VALUES (4, 4);
DELETE FROM dbo.Tables WHERE TableId = SCOPE_IDENTITY();
