WAITFOR DELAY '00:00:00.850';
SELECT ReservationId, ReservationDate, PartySize, CustomerFirstName, CustomerLastName, RestaurantName
FROM dbo.vw_ReservationDetails
ORDER BY ReservationDate;
