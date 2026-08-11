using Microsoft.Data.SqlClient;

namespace RestaurantReservation.Tests.Integration;

public sealed class DatabaseFixture : IAsyncLifetime
{
    private string _connectionString = string.Empty;

    public async Task InitializeAsync()
    {
        var container = await SqlServerContainer.InstanceAsync();

        _connectionString = new SqlConnectionStringBuilder(container.GetConnectionString())
        {
            InitialCatalog = $"Test_{Guid.NewGuid():N}"
        }.ConnectionString;

        await using var context = CreateContext();

        await context.Database.MigrateAsync();
    }

    public RestaurantReservationDbContext CreateContext() => new(new DbContextOptionsBuilder<RestaurantReservationDbContext>().UseSqlServer(_connectionString).Options);

    public Task DisposeAsync() => Task.CompletedTask;
}
