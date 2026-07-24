CREATE OR ALTER PROCEDURE dbo.sp_FindCustomersByPartySize (@PartySize INT)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT DISTINCT
        c.CustomerId,
        c.FirstName,
        c.LastName,
        c.Email,
        c.PhoneNumber
    FROM dbo.Customers AS c
    INNER JOIN dbo.Reservations AS r ON r.CustomerId = c.CustomerId
    WHERE r.PartySize > @PartySize;
END;
