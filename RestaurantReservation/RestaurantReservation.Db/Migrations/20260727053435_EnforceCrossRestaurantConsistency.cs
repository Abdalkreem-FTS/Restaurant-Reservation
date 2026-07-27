using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RestaurantReservation.Db.Migrations
{
    /// <inheritdoc />
    public partial class EnforceCrossRestaurantConsistency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_MenuItems_ItemId",
                table: "OrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Orders_OrderId",
                table: "OrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Employees_EmployeeId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Reservations_ReservationId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Tables_TableId",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_TableId",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Orders_EmployeeId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_ReservationId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_ItemId",
                table: "OrderItems");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems");

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "OrderId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "OrderId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "OrderId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "OrderId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "OrderId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "OrderId",
                keyValue: 6);

            migrationBuilder.AddColumn<int>(
                name: "RestaurantId",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RestaurantId",
                table: "OrderItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Tables_TableId_RestaurantId",
                table: "Tables",
                columns: new[] { "TableId", "RestaurantId" });

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Reservations_ReservationId_RestaurantId",
                table: "Reservations",
                columns: new[] { "ReservationId", "RestaurantId" });

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Orders_OrderId_RestaurantId",
                table: "Orders",
                columns: new[] { "OrderId", "RestaurantId" });

            migrationBuilder.AddUniqueConstraint(
                name: "AK_MenuItems_ItemId_RestaurantId",
                table: "MenuItems",
                columns: new[] { "ItemId", "RestaurantId" });

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Employees_EmployeeId_RestaurantId",
                table: "Employees",
                columns: new[] { "EmployeeId", "RestaurantId" });

            migrationBuilder.UpdateData(
                table: "OrderItems",
                keyColumn: "OrderItemId",
                keyValue: 1,
                column: "RestaurantId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "OrderItems",
                keyColumn: "OrderItemId",
                keyValue: 2,
                column: "RestaurantId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "OrderItems",
                keyColumn: "OrderItemId",
                keyValue: 3,
                column: "RestaurantId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "OrderItems",
                keyColumn: "OrderItemId",
                keyValue: 4,
                column: "RestaurantId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "OrderItems",
                keyColumn: "OrderItemId",
                keyValue: 5,
                column: "RestaurantId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "OrderItems",
                keyColumn: "OrderItemId",
                keyValue: 6,
                column: "RestaurantId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "OrderItems",
                keyColumn: "OrderItemId",
                keyValue: 7,
                column: "RestaurantId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "OrderItems",
                keyColumn: "OrderItemId",
                keyValue: 8,
                column: "RestaurantId",
                value: 4);

            migrationBuilder.InsertData(
                table: "Orders",
                columns: new[] { "OrderId", "EmployeeId", "OrderDate", "ReservationId", "RestaurantId", "TotalAmount" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2026, 8, 1, 19, 30, 0, 0, DateTimeKind.Unspecified), 1, 1, 37.49m },
                    { 2, 2, new DateTime(2026, 8, 1, 20, 0, 0, 0, DateTimeKind.Unspecified), 1, 1, 24.99m },
                    { 3, 1, new DateTime(2026, 8, 2, 20, 30, 0, 0, DateTimeKind.Unspecified), 2, 1, 49.98m },
                    { 4, 3, new DateTime(2026, 8, 3, 19, 0, 0, 0, DateTimeKind.Unspecified), 3, 2, 32.50m },
                    { 5, 5, new DateTime(2026, 8, 4, 20, 0, 0, 0, DateTimeKind.Unspecified), 4, 3, 29.99m },
                    { 6, 6, new DateTime(2026, 8, 5, 20, 30, 0, 0, DateTimeKind.Unspecified), 5, 4, 26.00m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_TableId_RestaurantId",
                table: "Reservations",
                columns: new[] { "TableId", "RestaurantId" });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_EmployeeId_RestaurantId",
                table: "Orders",
                columns: new[] { "EmployeeId", "RestaurantId" });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ReservationId_RestaurantId",
                table: "Orders",
                columns: new[] { "ReservationId", "RestaurantId" });

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ItemId_RestaurantId",
                table: "OrderItems",
                columns: new[] { "ItemId", "RestaurantId" });

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId_RestaurantId",
                table: "OrderItems",
                columns: new[] { "OrderId", "RestaurantId" });

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_MenuItems_ItemId_RestaurantId",
                table: "OrderItems",
                columns: new[] { "ItemId", "RestaurantId" },
                principalTable: "MenuItems",
                principalColumns: new[] { "ItemId", "RestaurantId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Orders_OrderId_RestaurantId",
                table: "OrderItems",
                columns: new[] { "OrderId", "RestaurantId" },
                principalTable: "Orders",
                principalColumns: new[] { "OrderId", "RestaurantId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Employees_EmployeeId_RestaurantId",
                table: "Orders",
                columns: new[] { "EmployeeId", "RestaurantId" },
                principalTable: "Employees",
                principalColumns: new[] { "EmployeeId", "RestaurantId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Reservations_ReservationId_RestaurantId",
                table: "Orders",
                columns: new[] { "ReservationId", "RestaurantId" },
                principalTable: "Reservations",
                principalColumns: new[] { "ReservationId", "RestaurantId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Tables_TableId_RestaurantId",
                table: "Reservations",
                columns: new[] { "TableId", "RestaurantId" },
                principalTable: "Tables",
                principalColumns: new[] { "TableId", "RestaurantId" },
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_MenuItems_ItemId_RestaurantId",
                table: "OrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Orders_OrderId_RestaurantId",
                table: "OrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Employees_EmployeeId_RestaurantId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Reservations_ReservationId_RestaurantId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Tables_TableId_RestaurantId",
                table: "Reservations");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Tables_TableId_RestaurantId",
                table: "Tables");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Reservations_ReservationId_RestaurantId",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_TableId_RestaurantId",
                table: "Reservations");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Orders_OrderId_RestaurantId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_EmployeeId_RestaurantId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_ReservationId_RestaurantId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_ItemId_RestaurantId",
                table: "OrderItems");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_OrderId_RestaurantId",
                table: "OrderItems");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_MenuItems_ItemId_RestaurantId",
                table: "MenuItems");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Employees_EmployeeId_RestaurantId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "RestaurantId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "RestaurantId",
                table: "OrderItems");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_TableId",
                table: "Reservations",
                column: "TableId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_EmployeeId",
                table: "Orders",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ReservationId",
                table: "Orders",
                column: "ReservationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ItemId",
                table: "OrderItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_MenuItems_ItemId",
                table: "OrderItems",
                column: "ItemId",
                principalTable: "MenuItems",
                principalColumn: "ItemId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Orders_OrderId",
                table: "OrderItems",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "OrderId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Employees_EmployeeId",
                table: "Orders",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Reservations_ReservationId",
                table: "Orders",
                column: "ReservationId",
                principalTable: "Reservations",
                principalColumn: "ReservationId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Tables_TableId",
                table: "Reservations",
                column: "TableId",
                principalTable: "Tables",
                principalColumn: "TableId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
