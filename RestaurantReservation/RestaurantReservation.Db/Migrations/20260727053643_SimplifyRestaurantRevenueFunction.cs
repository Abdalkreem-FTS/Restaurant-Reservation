using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestaurantReservation.Db.Migrations
{
    /// <inheritdoc />
    public partial class SimplifyRestaurantRevenueFunction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                                 CREATE OR ALTER FUNCTION dbo.fn_CalculateRestaurantRevenue (@restaurantId INT)
                                 RETURNS DECIMAL(18, 2)
                                 AS
                                 BEGIN
                                     DECLARE @total DECIMAL(18, 2);

                                     SELECT @total = ISNULL(SUM(o.TotalAmount), 0)
                                     FROM dbo.Orders AS o
                                     WHERE o.RestaurantId = @restaurantId;

                                     RETURN @total;
                                 END;
                                 """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
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
                                 """);
        }
    }
}
