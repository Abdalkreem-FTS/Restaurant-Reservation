INSERT INTO dbo.Tables (RestaurantId, Capacity)
VALUES (5, 4);
DELETE FROM dbo.Tables WHERE TableId = SCOPE_IDENTITY();
