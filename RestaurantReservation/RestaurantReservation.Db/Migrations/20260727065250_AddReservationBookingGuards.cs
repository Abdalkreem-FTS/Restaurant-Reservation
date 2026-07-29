using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestaurantReservation.Db.Migrations
{
    /// <inheritdoc />
    public partial class AddReservationBookingGuards : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Tables_TableId_RestaurantId",
                table: "Reservations");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Tables_TableId_RestaurantId",
                table: "Tables");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_TableId_RestaurantId",
                table: "Reservations");

            migrationBuilder.AddColumn<int>(
                name: "TableCapacity",
                table: "Reservations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Tables_TableId_RestaurantId_Capacity",
                table: "Tables",
                columns: new[] { "TableId", "RestaurantId", "Capacity" });

            migrationBuilder.UpdateData(
                table: "Reservations",
                keyColumn: "ReservationId",
                keyValue: 1,
                column: "TableCapacity",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Reservations",
                keyColumn: "ReservationId",
                keyValue: 2,
                column: "TableCapacity",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Reservations",
                keyColumn: "ReservationId",
                keyValue: 3,
                columns: new[] { "ReservationDate", "TableCapacity" },
                values: new object[] { new DateTime(2026, 8, 3, 18, 0, 0, 0, DateTimeKind.Unspecified), 4 });

            migrationBuilder.UpdateData(
                table: "Reservations",
                keyColumn: "ReservationId",
                keyValue: 4,
                columns: new[] { "ReservationDate", "TableCapacity" },
                values: new object[] { new DateTime(2026, 8, 4, 19, 0, 0, 0, DateTimeKind.Unspecified), 6 });

            migrationBuilder.UpdateData(
                table: "Reservations",
                keyColumn: "ReservationId",
                keyValue: 5,
                column: "TableCapacity",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Reservations",
                keyColumn: "ReservationId",
                keyValue: 6,
                column: "TableCapacity",
                value: 8);

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_TableId_ReservationDate",
                table: "Reservations",
                columns: new[] { "TableId", "ReservationDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_TableId_RestaurantId_TableCapacity",
                table: "Reservations",
                columns: new[] { "TableId", "RestaurantId", "TableCapacity" });

            migrationBuilder.AddCheckConstraint(
                name: "CK_Reservations_PartySizeIsPositive",
                table: "Reservations",
                sql: "[PartySize] > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Reservations_PartySizeWithinTableCapacity",
                table: "Reservations",
                sql: "[PartySize] <= [TableCapacity]");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Reservations_ReservationDateOnTheHour",
                table: "Reservations",
                sql: "DATEPART(MINUTE, [ReservationDate]) = 0 AND DATEPART(SECOND, [ReservationDate]) = 0 AND DATEPART(NANOSECOND, [ReservationDate]) = 0");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Tables_TableId_RestaurantId_TableCapacity",
                table: "Reservations",
                columns: new[] { "TableId", "RestaurantId", "TableCapacity" },
                principalTable: "Tables",
                principalColumns: new[] { "TableId", "RestaurantId", "Capacity" },
                onUpdate: ReferentialAction.Cascade,
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Tables_TableId_RestaurantId_TableCapacity",
                table: "Reservations");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Tables_TableId_RestaurantId_Capacity",
                table: "Tables");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_TableId_ReservationDate",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_TableId_RestaurantId_TableCapacity",
                table: "Reservations");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Reservations_PartySizeIsPositive",
                table: "Reservations");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Reservations_PartySizeWithinTableCapacity",
                table: "Reservations");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Reservations_ReservationDateOnTheHour",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "TableCapacity",
                table: "Reservations");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Tables_TableId_RestaurantId",
                table: "Tables",
                columns: new[] { "TableId", "RestaurantId" });

            migrationBuilder.UpdateData(
                table: "Reservations",
                keyColumn: "ReservationId",
                keyValue: 3,
                column: "ReservationDate",
                value: new DateTime(2026, 8, 3, 18, 30, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Reservations",
                keyColumn: "ReservationId",
                keyValue: 4,
                column: "ReservationDate",
                value: new DateTime(2026, 8, 4, 19, 30, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_TableId_RestaurantId",
                table: "Reservations",
                columns: new[] { "TableId", "RestaurantId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Tables_TableId_RestaurantId",
                table: "Reservations",
                columns: new[] { "TableId", "RestaurantId" },
                principalTable: "Tables",
                principalColumns: new[] { "TableId", "RestaurantId" },
                onDelete: ReferentialAction.Restrict);
        }
    }
}
