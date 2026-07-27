using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestaurantReservation.Db.Migrations
{
    /// <inheritdoc />
    public partial class AddValueRangeGuards : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddCheckConstraint(
                name: "CK_Tables_CapacityIsPositive",
                table: "Tables",
                sql: "[Capacity] > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_OrderItems_QuantityIsPositive",
                table: "OrderItems",
                sql: "[Quantity] > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_MenuItems_PriceIsNotNegative",
                table: "MenuItems",
                sql: "[Price] >= 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Tables_CapacityIsPositive",
                table: "Tables");

            migrationBuilder.DropCheckConstraint(
                name: "CK_OrderItems_QuantityIsPositive",
                table: "OrderItems");

            migrationBuilder.DropCheckConstraint(
                name: "CK_MenuItems_PriceIsNotNegative",
                table: "MenuItems");
        }
    }
}
