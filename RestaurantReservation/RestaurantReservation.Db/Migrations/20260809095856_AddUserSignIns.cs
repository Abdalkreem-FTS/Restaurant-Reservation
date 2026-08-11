using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RestaurantReservation.Db.Migrations
{
    /// <inheritdoc />
    public partial class AddUserSignIns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Employees",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Customers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 1,
                column: "UserId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 2,
                column: "UserId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 3,
                column: "UserId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 4,
                column: "UserId",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 5,
                column: "UserId",
                value: 5);

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 1,
                column: "UserId",
                value: 6);

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 2,
                column: "UserId",
                value: 7);

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 3,
                column: "UserId",
                value: 8);

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 4,
                column: "UserId",
                value: 9);

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 5,
                column: "UserId",
                value: 10);

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 6,
                column: "UserId",
                value: 11);

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "PasswordHash", "Username" },
                values: new object[,]
                {
                    { 1, "AQAAAAIAAYagAAAAEIBPg0ewhJ5n53xz44KlYSSDvhwxyBuRtzQ+ytXmpEao5y/dmWmFOp9jKOJa7ux+bQ==", "john.doe" },
                    { 2, "AQAAAAIAAYagAAAAEDm7Y7HkVS/kaIjfrvDj74Oca+8J1GYa7mpUjuUg2ag4HQvo4Jk6tIm47/VGnbmkyw==", "jane.smith" },
                    { 3, "AQAAAAIAAYagAAAAEKBdwZsSAeO6CJrYUhTx74qhMv3YxDO/loMRtHxIs7XFN51qIocdI9jRe0Q98PYOTQ==", "michael.johnson" },
                    { 4, "AQAAAAIAAYagAAAAEOTo3Ly50M1eu49u3Mf9qr+NkRO2QRWcNRjZDqBUfNRLz0gEgCsSS4dm4a7B6TCJJg==", "emily.davis" },
                    { 5, "AQAAAAIAAYagAAAAEBKJGhoqk3DWchXBg6SQULteviXT1GatQtEXHqWnto6/LBam3IdGQWDibFUgf/yk1w==", "david.wilson" },
                    { 6, "AQAAAAIAAYagAAAAENo/A93E/1FhZg8+OAnOLm70xAqSjW6clyn4AJIpCH4GcsBwuJaxf5UEA2oZ4UQjSQ==", "alice.turner" },
                    { 7, "AQAAAAIAAYagAAAAEExphGBVuZExPZTlHQ6hQO1GT+nyON6koGot2siIyRA1lE1WzPDn0h1xXAXbpR3Yjw==", "bob.cook" },
                    { 8, "AQAAAAIAAYagAAAAELVW31dCx40O1GBijQ7Upeu9sGZVfCHyQhBLqGR7P9vXO0hcsmrRNx8uuhNB2zQ61Q==", "carol.white" },
                    { 9, "AQAAAAIAAYagAAAAEDrJQV8EwE1UtxJtz6GE2j0AHcNTlzBiFcKt8cwXwWC3oBHA95ImfdMgaeXnZs8IRQ==", "dan.brown" },
                    { 10, "AQAAAAIAAYagAAAAEMgwF8w+eAczEKjurnAoIhjX+BxqRpaZxALyFd3XRU3UIcMI/MVHMQRmLPwmkDSX5Q==", "eve.black" },
                    { 11, "AQAAAAIAAYagAAAAEP0ocSpAttnNpNcwKkhuNIFho6xB3NNWy/FFZp/XaKuS5p3uUkeQ3DfjfmQ/klby4g==", "frank.green" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_UserId",
                table: "Employees",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_UserId",
                table: "Customers",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_Users_UserId",
                table: "Customers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Users_UserId",
                table: "Employees",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            // FromSql materializes a Customer, so the procedure has to return every column the entity
            // now maps. Without UserId it fails with "The required column 'UserId' was not present".
            migrationBuilder.Sql("""
                                 CREATE OR ALTER PROCEDURE dbo.sp_FindCustomersByPartySize
                                     @PartySize INT,
                                     @Offset INT = 0,
                                     @PageSize INT = 20
                                 AS
                                 BEGIN
                                     SET NOCOUNT ON;

                                     SELECT DISTINCT
                                         c.CustomerId,
                                         c.UserId,
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
                                 """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
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
                                 """);

            migrationBuilder.DropForeignKey(
                name: "FK_Customers_Users_UserId",
                table: "Customers");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Users_UserId",
                table: "Employees");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Employees_UserId",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Customers_UserId",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Customers");
        }
    }
}
