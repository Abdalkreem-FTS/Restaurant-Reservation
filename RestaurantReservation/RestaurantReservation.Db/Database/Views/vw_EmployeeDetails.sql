CREATE OR ALTER VIEW dbo.vw_EmployeeDetails
AS
SELECT
    e.EmployeeId,
    e.FirstName,
    e.LastName,
    e.Position,
    rest.RestaurantId,
    rest.Name          AS RestaurantName,
    rest.Address       AS RestaurantAddress,
    rest.PhoneNumber   AS RestaurantPhoneNumber,
    rest.OpeningHours  AS RestaurantOpeningHours
FROM dbo.Employees AS e
INNER JOIN dbo.Restaurants AS rest ON rest.RestaurantId = e.RestaurantId;
