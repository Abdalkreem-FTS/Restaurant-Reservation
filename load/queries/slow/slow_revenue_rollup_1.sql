WAITFOR DELAY '00:00:00.700';
SELECT r.RestaurantId, r.Name, dbo.fn_CalculateRestaurantRevenue(r.RestaurantId) AS Revenue
FROM dbo.Restaurants AS r
ORDER BY Revenue DESC;
