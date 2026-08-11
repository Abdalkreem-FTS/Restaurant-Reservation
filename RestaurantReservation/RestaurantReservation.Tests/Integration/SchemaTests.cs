namespace RestaurantReservation.Tests.Integration;

public class SchemaTests(DatabaseFixture fixture) : DatabaseTest(fixture), IClassFixture<DatabaseFixture>
{
    [Fact]
    public async Task Migrate_OnAFreshDatabase_AppliesInitialCreateAndLeavesNothingPending()
    {
        var applied = await Context.Database.GetAppliedMigrationsAsync();

        Assert.Contains(applied, migration => migration.EndsWith("InitialCreate", StringComparison.Ordinal));
        Assert.Empty(await Context.Database.GetPendingMigrationsAsync());
    }
    
    [Fact]
    public async Task Migrate_ForTheReservationToTableForeignKey_CascadesUpdatesButStillRestrictsDeletes()
    {
        const string foreignKey = "FK_Reservations_Tables_TableId_RestaurantId_TableCapacity";

        var onUpdate = await Context.Database
            .SqlQuery<int>($"SELECT CAST(fk.update_referential_action AS int) AS Value FROM sys.foreign_keys fk WHERE fk.name = {foreignKey}")
            .ToListAsync();

        var onDelete = await Context.Database
            .SqlQuery<int>($"SELECT CAST(fk.delete_referential_action AS int) AS Value FROM sys.foreign_keys fk WHERE fk.name = {foreignKey}")
            .ToListAsync();

        Assert.Equal(1, Assert.Single(onUpdate));
        Assert.Equal(0, Assert.Single(onDelete));
    }

    [Fact]
    public async Task Migrate_OnAFreshDatabase_SeedsEveryTableWithAtLeastFiveRows()
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
    public async Task Seed_ForEveryOrder_TotalAmountMatchesItsOrderItems()
    {
        var mismatched = await Context.Orders
            .Where(order => order.TotalAmount != order.OrderItems.Sum(item => item.UnitPrice * item.Quantity))
            .Select(order => order.OrderId)
            .ToListAsync();

        Assert.Empty(mismatched);
    }

    [Fact]
    public async Task Migrate_OnAFreshDatabase_StoresEmployeePositionsAsReadableStrings()
    {
        var stored = await Context.Database
            .SqlQuery<string>($"SELECT DISTINCT e.Position AS Value FROM dbo.Employees e")
            .ToListAsync();

        Assert.NotEmpty(stored);
        Assert.All(stored, position => Assert.True(
            Enum.TryParse<EmployeePosition>(position, ignoreCase: false, out _),
            $"'{position}' is not an EmployeePosition name"));
    }
}
