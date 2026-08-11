SELECT PartySize, COUNT(*) AS Reservations
FROM dbo.Reservations
GROUP BY PartySize
ORDER BY PartySize;
