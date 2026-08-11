using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RestaurantReservation.Db.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    CustomerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(254)", maxLength: 254, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_Customers", x => x.CustomerId); });

            migrationBuilder.CreateTable(
                name: "Restaurants",
                columns: table => new
                {
                    RestaurantId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    OpeningHours = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_Restaurants", x => x.RestaurantId); });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    EmployeeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RestaurantId = table.Column<int>(type: "int", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Position = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.EmployeeId);
                    table.ForeignKey(
                        name: "FK_Employees_Restaurants_RestaurantId",
                        column: x => x.RestaurantId,
                        principalTable: "Restaurants",
                        principalColumn: "RestaurantId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MenuItems",
                columns: table => new
                {
                    ItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RestaurantId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuItems", x => x.ItemId);
                    table.ForeignKey(
                        name: "FK_MenuItems_Restaurants_RestaurantId",
                        column: x => x.RestaurantId,
                        principalTable: "Restaurants",
                        principalColumn: "RestaurantId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Tables",
                columns: table => new
                {
                    TableId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RestaurantId = table.Column<int>(type: "int", nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tables", x => x.TableId);
                    table.ForeignKey(
                        name: "FK_Tables_Restaurants_RestaurantId",
                        column: x => x.RestaurantId,
                        principalTable: "Restaurants",
                        principalColumn: "RestaurantId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Reservations",
                columns: table => new
                {
                    ReservationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    RestaurantId = table.Column<int>(type: "int", nullable: false),
                    TableId = table.Column<int>(type: "int", nullable: false),
                    ReservationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PartySize = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservations", x => x.ReservationId);
                    table.ForeignKey(
                        name: "FK_Reservations_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reservations_Restaurants_RestaurantId",
                        column: x => x.RestaurantId,
                        principalTable: "Restaurants",
                        principalColumn: "RestaurantId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reservations_Tables_TableId",
                        column: x => x.TableId,
                        principalTable: "Tables",
                        principalColumn: "TableId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    OrderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReservationId = table.Column<int>(type: "int", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.OrderId);
                    table.ForeignKey(
                        name: "FK_Orders_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orders_Reservations_ReservationId",
                        column: x => x.ReservationId,
                        principalTable: "Reservations",
                        principalColumn: "ReservationId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    OrderItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.OrderItemId);
                    table.ForeignKey(
                        name: "FK_OrderItems_MenuItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "MenuItems",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "CustomerId", "Email", "FirstName", "LastName", "PhoneNumber" },
                values: new object[,]
                {
                    { 1, "john.doe@example.com", "John", "Doe", "555-1001" },
                    { 2, "jane.smith@example.com", "Jane", "Smith", "555-1002" },
                    { 3, "michael.johnson@example.com", "Michael", "Johnson", "555-1003" },
                    { 4, "emily.davis@example.com", "Emily", "Davis", "555-1004" },
                    { 5, "david.wilson@example.com", "David", "Wilson", "555-1005" }
                });

            migrationBuilder.InsertData(
                table: "Restaurants",
                columns: new[] { "RestaurantId", "Address", "Name", "OpeningHours", "PhoneNumber" },
                values: new object[,]
                {
                    { 1, "123 Main St, Springfield", "The Gourmet Kitchen", "10:00-23:00", "555-0101" },
                    { 2, "45 Oak Avenue, Springfield", "Bella Italia", "11:00-22:00", "555-0102" },
                    { 3, "9 River Road, Shelbyville", "Sushi Zen", "12:00-22:30", "555-0103" },
                    { 4, "78 Elm Street, Shelbyville", "Le Petite Bistro", "17:00-23:30", "555-0104" },
                    { 5, "200 Grand Blvd, Capital City", "The Steakhouse", "16:00-00:00", "555-0105" }
                });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "EmployeeId", "FirstName", "LastName", "Position", "RestaurantId" },
                values: new object[,]
                {
                    { 1, "Alice", "Turner", "Manager", 1 },
                    { 2, "Bob", "Cook", "VipOrdersWaiter", 1 },
                    { 3, "Carol", "White", "Manager", 2 },
                    { 4, "Dan", "Brown", "StandardWaiter", 2 },
                    { 5, "Eve", "Black", "Manager", 3 },
                    { 6, "Frank", "Green", "AssistantWaiter", 4 }
                });

            migrationBuilder.InsertData(
                table: "MenuItems",
                columns: new[] { "ItemId", "Description", "Name", "Price", "RestaurantId" },
                values: new object[,]
                {
                    { 1, "Atlantic salmon with lemon butter", "Grilled Salmon", 24.99m, 1 },
                    { 2, "Romaine, parmesan, croutons", "Caesar Salad", 12.50m, 1 },
                    { 3, "Tomato, mozzarella, basil", "Margherita Pizza", 15.00m, 2 },
                    { 4, "Egg, pancetta, pecorino", "Spaghetti Carbonara", 17.50m, 2 },
                    { 5, "Chef's selection of 12 pieces", "Sushi Platter", 29.99m, 3 },
                    { 6, "Slow-braised beef in red wine", "Beef Bourguignon", 26.00m, 4 }
                });

            migrationBuilder.InsertData(
                table: "Tables",
                columns: new[] { "TableId", "Capacity", "RestaurantId" },
                values: new object[,]
                {
                    { 1, 2, 1 },
                    { 2, 4, 1 },
                    { 3, 4, 2 },
                    { 4, 6, 3 },
                    { 5, 2, 4 },
                    { 6, 8, 5 }
                });

            migrationBuilder.InsertData(
                table: "Reservations",
                columns: new[]
                    { "ReservationId", "CustomerId", "PartySize", "ReservationDate", "RestaurantId", "TableId" },
                values: new object[,]
                {
                    { 1, 1, 2, new DateTime(2026, 8, 1, 19, 0, 0, 0, DateTimeKind.Unspecified), 1, 1 },
                    { 2, 2, 4, new DateTime(2026, 8, 2, 20, 0, 0, 0, DateTimeKind.Unspecified), 1, 2 },
                    { 3, 3, 3, new DateTime(2026, 8, 3, 18, 30, 0, 0, DateTimeKind.Unspecified), 2, 3 },
                    { 4, 4, 6, new DateTime(2026, 8, 4, 19, 30, 0, 0, DateTimeKind.Unspecified), 3, 4 },
                    { 5, 5, 2, new DateTime(2026, 8, 5, 20, 0, 0, 0, DateTimeKind.Unspecified), 4, 5 },
                    { 6, 1, 8, new DateTime(2026, 8, 6, 21, 0, 0, 0, DateTimeKind.Unspecified), 5, 6 }
                });

            migrationBuilder.InsertData(
                table: "Orders",
                columns: new[] { "OrderId", "EmployeeId", "OrderDate", "ReservationId", "TotalAmount" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2026, 8, 1, 19, 30, 0, 0, DateTimeKind.Unspecified), 1, 37.49m },
                    { 2, 2, new DateTime(2026, 8, 1, 20, 0, 0, 0, DateTimeKind.Unspecified), 1, 24.99m },
                    { 3, 1, new DateTime(2026, 8, 2, 20, 30, 0, 0, DateTimeKind.Unspecified), 2, 49.98m },
                    { 4, 3, new DateTime(2026, 8, 3, 19, 0, 0, 0, DateTimeKind.Unspecified), 3, 32.50m },
                    { 5, 5, new DateTime(2026, 8, 4, 20, 0, 0, 0, DateTimeKind.Unspecified), 4, 29.99m },
                    { 6, 6, new DateTime(2026, 8, 5, 20, 30, 0, 0, DateTimeKind.Unspecified), 5, 26.00m }
                });

            migrationBuilder.InsertData(
                table: "OrderItems",
                columns: new[] { "OrderItemId", "ItemId", "OrderId", "Quantity" },
                values: new object[,]
                {
                    { 1, 1, 1, 1 },
                    { 2, 2, 1, 1 },
                    { 3, 1, 2, 1 },
                    { 4, 1, 3, 2 },
                    { 5, 3, 4, 1 },
                    { 6, 4, 4, 1 },
                    { 7, 5, 5, 1 },
                    { 8, 6, 6, 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Email",
                table: "Customers",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_RestaurantId",
                table: "Employees",
                column: "RestaurantId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuItems_RestaurantId",
                table: "MenuItems",
                column: "RestaurantId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ItemId",
                table: "OrderItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_EmployeeId",
                table: "Orders",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ReservationId",
                table: "Orders",
                column: "ReservationId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_CustomerId",
                table: "Reservations",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_RestaurantId",
                table: "Reservations",
                column: "RestaurantId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_TableId",
                table: "Reservations",
                column: "TableId");

            migrationBuilder.CreateIndex(
                name: "IX_Tables_RestaurantId",
                table: "Tables",
                column: "RestaurantId");

            migrationBuilder.Sql("""
                                 CREATE OR ALTER VIEW dbo.vw_ReservationDetails AS
                                 SELECT r.ReservationId, r.ReservationDate, r.PartySize,
                                        c.CustomerId, c.FirstName AS CustomerFirstName, c.LastName AS CustomerLastName,
                                        c.Email AS CustomerEmail, c.PhoneNumber AS CustomerPhoneNumber,
                                        rest.RestaurantId, rest.Name AS RestaurantName, rest.Address AS RestaurantAddress,
                                        rest.PhoneNumber AS RestaurantPhoneNumber
                                 FROM dbo.Reservations r
                                 JOIN dbo.Customers c ON c.CustomerId = r.CustomerId
                                 JOIN dbo.Restaurants rest ON rest.RestaurantId = r.RestaurantId;
                                 """);
            migrationBuilder.Sql("""
                                 CREATE OR ALTER VIEW dbo.vw_EmployeeDetails AS
                                 SELECT e.EmployeeId, e.FirstName, e.LastName, e.Position,
                                        rest.RestaurantId, rest.Name AS RestaurantName, rest.Address AS RestaurantAddress,
                                        rest.PhoneNumber AS RestaurantPhoneNumber, rest.OpeningHours AS RestaurantOpeningHours
                                 FROM dbo.Employees e
                                 JOIN dbo.Restaurants rest ON rest.RestaurantId = e.RestaurantId;
                                 """);
            migrationBuilder.Sql("""
                                 CREATE OR ALTER FUNCTION dbo.fn_CalculateRestaurantRevenue (@restaurantId INT)
                                 RETURNS DECIMAL(18,2) AS
                                 BEGIN
                                     DECLARE @total DECIMAL(18,2);
                                     SELECT @total = ISNULL(SUM(o.TotalAmount), 0)
                                     FROM dbo.Orders o
                                     JOIN dbo.Reservations r ON r.ReservationId = o.ReservationId
                                     WHERE r.RestaurantId = @restaurantId;
                                     RETURN @total;
                                 END;
                                 """);
            migrationBuilder.Sql("""
                                 CREATE OR ALTER PROCEDURE dbo.sp_FindCustomersByPartySize (@PartySize INT) AS
                                 BEGIN
                                     SET NOCOUNT ON;
                                     SELECT DISTINCT c.CustomerId, c.FirstName, c.LastName, c.Email, c.PhoneNumber
                                     FROM dbo.Customers c
                                     JOIN dbo.Reservations r ON r.CustomerId = c.CustomerId
                                     WHERE r.PartySize > @PartySize;
                                 END;
                                 """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS dbo.sp_FindCustomersByPartySize;");
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS dbo.fn_CalculateRestaurantRevenue;");
            migrationBuilder.Sql("DROP VIEW IF EXISTS dbo.vw_EmployeeDetails;");
            migrationBuilder.Sql("DROP VIEW IF EXISTS dbo.vw_ReservationDetails;");

            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "MenuItems");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "Reservations");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "Tables");

            migrationBuilder.DropTable(
                name: "Restaurants");
        }
    }
}
