CREATE OR ALTER PROCEDURE dbo.sp_FindCustomersByPartySize
    @PartySize INT,
    @Offset INT = 0,
    @PageSize INT = 20
AS
BEGIN
    SET NOCOUNT ON;

    SELECT DISTINCT
        c.CustomerId,
        c.FirstName,
        c.LastName,
        c.Email,
        c.PhoneNumber,
        c.RowVersion
    FROM dbo.Customers AS c
    INNER JOIN dbo.Reservations AS r ON r.CustomerId = c.CustomerId
    WHERE r.PartySize > @PartySize
    ORDER BY c.CustomerId
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
END;
