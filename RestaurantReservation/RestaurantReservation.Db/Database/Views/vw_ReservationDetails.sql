CREATE OR ALTER VIEW dbo.vw_ReservationDetails
AS
SELECT
    r.ReservationId,
    r.ReservationDate,
    r.PartySize,
    c.CustomerId,
    c.FirstName        AS CustomerFirstName,
    c.LastName         AS CustomerLastName,
    c.Email            AS CustomerEmail,
    c.PhoneNumber      AS CustomerPhoneNumber,
    rest.RestaurantId,
    rest.Name          AS RestaurantName,
    rest.Address       AS RestaurantAddress,
    rest.PhoneNumber   AS RestaurantPhoneNumber
FROM dbo.Reservations AS r
INNER JOIN dbo.Customers   AS c    ON c.CustomerId   = r.CustomerId
INNER JOIN dbo.Restaurants AS rest ON rest.RestaurantId = r.RestaurantId;
