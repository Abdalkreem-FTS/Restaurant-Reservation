INSERT INTO dbo.Tables (RestaurantId, Capacity)
VALUES (2, 4);
DELETE FROM dbo.Tables WHERE TableId = SCOPE_IDENTITY();
