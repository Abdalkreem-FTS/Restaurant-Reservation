using RestaurantReservation.Db.Enums;

namespace RestaurantReservation.Tests.Integration;

[Collection(DatabaseCollection.SqlServerDatabase)]
public class SchemaTests(SqlServerFixture fixture) : DatabaseTest(fixture)
{
    [Fact]
    public async Task MigrateAsync_OnAFreshDatabase_AppliesInitialCreateAndLeavesNothingPending()
    {
        var applied = await Context.Database.GetAppliedMigrationsAsync();

        Assert.Contains(applied, migration => migration.EndsWith("InitialCreate", StringComparison.Ordinal));
        Assert.Empty(await Context.Database.GetPendingMigrationsAsync());
    }

    [Theory]
    [InlineData("Restaurants", "U")]
    [InlineData("Customers", "U")]
    [InlineData("Tables", "U")]
    [InlineData("Employees", "U")]
    [InlineData("MenuItems", "U")]
    [InlineData("Reservations", "U")]
    [InlineData("Orders", "U")]
    [InlineData("OrderItems", "U")]
    [InlineData("vw_ReservationDetails", "V")]
    [InlineData("vw_EmployeeDetails", "V")]
    [InlineData("fn_CalculateRestaurantRevenue", "FN")]
    [InlineData("sp_FindCustomersByPartySize", "P")]
    public async Task MigrateAsync_OnAFreshDatabase_CreatesTheDatabaseObject(string name, string type)
    {
        var found = await Context.Database
            .SqlQuery<string>($"SELECT RTRIM(o.type) AS Value FROM sys.objects o WHERE o.name = {name}")
            .ToListAsync();

        Assert.Equal(type, Assert.Single(found));
    }

    [Fact]
    public async Task MigrateAsync_OnAFreshDatabase_SeedsEveryTableWithAtLeastFiveRows()
    {
        Assert.True(await Context.Restaurants.CountAsync() >= 5, "Restaurants");
        Assert.True(await Context.Customers.CountAsync() >= 5, "Customers");
        Assert.True(await Context.Tables.CountAsync() >= 5, "Tables");
        Assert.True(await Context.Employees.CountAsync() >= 5, "Employees");
        Assert.True(await Context.MenuItems.CountAsync() >= 5, "MenuItems");
        Assert.True(await Context.Reservations.CountAsync() >= 5, "Reservations");
        Assert.True(await Context.Orders.CountAsync() >= 5, "Orders");
        Assert.True(await Context.OrderItems.CountAsync() >= 5, "OrderItems");
    }

    [Fact]
    public async Task MigrateAsync_OnAFreshDatabase_StoresEmployeePositionsAsReadableStrings()
    {
        var stored = await Context.Database
            .SqlQuery<string>($"SELECT DISTINCT e.Position AS Value FROM dbo.Employees e")
            .ToListAsync();

        Assert.NotEmpty(stored);
        Assert.All(stored, position => Assert.True(
            Enum.TryParse<EmployeePosition>(position, ignoreCase: false, out _),
            $"'{position}' is not an EmployeePosition name"));
    }

    [Fact]
    public async Task MigrateAsync_OnAFreshDatabase_CreatesMoneyColumnsWithTwoDecimalPlaces()
    {
        var scale = await Context.Database
            .SqlQuery<int>(
                $"""
                 SELECT CAST(c.scale AS int) AS Value
                 FROM sys.columns c
                 WHERE c.object_id = OBJECT_ID('dbo.Orders') AND c.name = 'TotalAmount'
                 """)
            .ToListAsync();

        Assert.Equal(2, Assert.Single(scale));
    }
}
