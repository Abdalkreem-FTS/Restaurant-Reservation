CREATE OR ALTER FUNCTION dbo.fn_CalculateRestaurantRevenue (@restaurantId INT)
RETURNS DECIMAL(18, 2)
AS
BEGIN
    DECLARE @total DECIMAL(18, 2);

    SELECT @total = ISNULL(SUM(o.TotalAmount), 0)
    FROM dbo.Orders AS o
    INNER JOIN dbo.Reservations AS r ON r.ReservationId = o.ReservationId
    WHERE r.RestaurantId = @restaurantId;

    RETURN @total;
END;
