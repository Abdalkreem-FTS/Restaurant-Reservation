using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestaurantReservation.Db.Migrations
{
    /// <inheritdoc />
    public partial class UseDateTimeOffsetForDates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Reservations_ReservationDateOnTheHour",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_TableId_ReservationDate",
                table: "Reservations");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "ReservationDate",
                table: "Reservations",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_TableId_ReservationDate",
                table: "Reservations",
                columns: ["TableId", "ReservationDate"],
                unique: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Reservations_ReservationDateOnTheHour",
                table: "Reservations",
                sql: "DATEPART(MINUTE, [ReservationDate]) = 0 AND DATEPART(SECOND, [ReservationDate]) = 0 AND DATEPART(NANOSECOND, [ReservationDate]) = 0");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "OrderDate",
                table: "Orders",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "OrderId",
                keyValue: 1,
                column: "OrderDate",
                value: new DateTimeOffset(new DateTime(2026, 8, 1, 19, 30, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "OrderId",
                keyValue: 2,
                column: "OrderDate",
                value: new DateTimeOffset(new DateTime(2026, 8, 1, 20, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "OrderId",
                keyValue: 3,
                column: "OrderDate",
                value: new DateTimeOffset(new DateTime(2026, 8, 2, 20, 30, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "OrderId",
                keyValue: 4,
                column: "OrderDate",
                value: new DateTimeOffset(new DateTime(2026, 8, 3, 19, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "OrderId",
                keyValue: 5,
                column: "OrderDate",
                value: new DateTimeOffset(new DateTime(2026, 8, 4, 20, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "OrderId",
                keyValue: 6,
                column: "OrderDate",
                value: new DateTimeOffset(new DateTime(2026, 8, 5, 20, 30, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Reservations",
                keyColumn: "ReservationId",
                keyValue: 1,
                column: "ReservationDate",
                value: new DateTimeOffset(new DateTime(2026, 8, 1, 19, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Reservations",
                keyColumn: "ReservationId",
                keyValue: 2,
                column: "ReservationDate",
                value: new DateTimeOffset(new DateTime(2026, 8, 2, 20, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Reservations",
                keyColumn: "ReservationId",
                keyValue: 3,
                column: "ReservationDate",
                value: new DateTimeOffset(new DateTime(2026, 8, 3, 18, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Reservations",
                keyColumn: "ReservationId",
                keyValue: 4,
                column: "ReservationDate",
                value: new DateTimeOffset(new DateTime(2026, 8, 4, 19, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Reservations",
                keyColumn: "ReservationId",
                keyValue: 5,
                column: "ReservationDate",
                value: new DateTimeOffset(new DateTime(2026, 8, 5, 20, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Reservations",
                keyColumn: "ReservationId",
                keyValue: 6,
                column: "ReservationDate",
                value: new DateTimeOffset(new DateTime(2026, 8, 6, 21, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
                    // Non-schemabound views cache their column metadata, so the view keeps reporting datetime2
            // until it is recompiled against the new column type.
            migrationBuilder.Sql("""
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
                                 """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Reservations_ReservationDateOnTheHour",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_TableId_ReservationDate",
                table: "Reservations");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ReservationDate",
                table: "Reservations",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_TableId_ReservationDate",
                table: "Reservations",
                columns: ["TableId", "ReservationDate"],
                unique: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Reservations_ReservationDateOnTheHour",
                table: "Reservations",
                sql: "DATEPART(MINUTE, [ReservationDate]) = 0 AND DATEPART(SECOND, [ReservationDate]) = 0 AND DATEPART(NANOSECOND, [ReservationDate]) = 0");

            migrationBuilder.AlterColumn<DateTime>(
                name: "OrderDate",
                table: "Orders",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset");

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "OrderId",
                keyValue: 1,
                column: "OrderDate",
                value: new DateTime(2026, 8, 1, 19, 30, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "OrderId",
                keyValue: 2,
                column: "OrderDate",
                value: new DateTime(2026, 8, 1, 20, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "OrderId",
                keyValue: 3,
                column: "OrderDate",
                value: new DateTime(2026, 8, 2, 20, 30, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "OrderId",
                keyValue: 4,
                column: "OrderDate",
                value: new DateTime(2026, 8, 3, 19, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "OrderId",
                keyValue: 5,
                column: "OrderDate",
                value: new DateTime(2026, 8, 4, 20, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "OrderId",
                keyValue: 6,
                column: "OrderDate",
                value: new DateTime(2026, 8, 5, 20, 30, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Reservations",
                keyColumn: "ReservationId",
                keyValue: 1,
                column: "ReservationDate",
                value: new DateTime(2026, 8, 1, 19, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Reservations",
                keyColumn: "ReservationId",
                keyValue: 2,
                column: "ReservationDate",
                value: new DateTime(2026, 8, 2, 20, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Reservations",
                keyColumn: "ReservationId",
                keyValue: 3,
                column: "ReservationDate",
                value: new DateTime(2026, 8, 3, 18, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Reservations",
                keyColumn: "ReservationId",
                keyValue: 4,
                column: "ReservationDate",
                value: new DateTime(2026, 8, 4, 19, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Reservations",
                keyColumn: "ReservationId",
                keyValue: 5,
                column: "ReservationDate",
                value: new DateTime(2026, 8, 5, 20, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Reservations",
                keyColumn: "ReservationId",
                keyValue: 6,
                column: "ReservationDate",
                value: new DateTime(2026, 8, 6, 21, 0, 0, 0, DateTimeKind.Unspecified));
                    // Non-schemabound views cache their column metadata, so the view keeps reporting datetime2
            // until it is recompiled against the new column type.
            migrationBuilder.Sql("""
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
                                 """);
        }
    }
}
