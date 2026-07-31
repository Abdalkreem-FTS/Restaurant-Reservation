SELECT rest.Name, AVG(CAST(r.PartySize AS DECIMAL(10, 2))) AS AveragePartySize
FROM dbo.Reservations AS r
INNER JOIN dbo.Restaurants AS rest ON rest.RestaurantId = r.RestaurantId
GROUP BY rest.Name
ORDER BY AveragePartySize DESC;
