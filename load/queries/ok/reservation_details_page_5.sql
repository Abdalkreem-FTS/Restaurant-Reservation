SELECT ReservationId, ReservationDate, PartySize, CustomerFirstName, CustomerLastName, RestaurantName
FROM dbo.vw_ReservationDetails
ORDER BY ReservationId
OFFSET 12 ROWS FETCH NEXT 3 ROWS ONLY;
