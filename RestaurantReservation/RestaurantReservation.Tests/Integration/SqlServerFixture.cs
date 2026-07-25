using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Testcontainers.MsSql;

namespace RestaurantReservation.Tests.Integration;

public sealed class SqlServerFixture : IAsyncLifetime
{
    private static readonly string Image = new ConfigurationBuilder()
        .SetBasePath(AppContext.BaseDirectory)
        .AddJsonFile("appsettings.json")
        .Build()["SqlServer:Image"]
        ?? throw new InvalidOperationException("\"SqlServer:Image\" is missing from appsettings.json.");

    private readonly MsSqlContainer _container = new MsSqlBuilder(Image).Build();

    private string _connectionString = string.Empty;

    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        _connectionString = new SqlConnectionStringBuilder(_container.GetConnectionString())
        {
            InitialCatalog = "RestaurantReservationCore"
        }.ConnectionString;

        await using var context = CreateContext();

        await context.Database.MigrateAsync();
    }

    public RestaurantReservationDbContext CreateContext() => new(new DbContextOptionsBuilder<RestaurantReservationDbContext>().UseSqlServer(_connectionString).Options);

    public async Task DisposeAsync() => await _container.DisposeAsync();
}

/// <summary>Shares the single container across every integration test class.</summary>
[CollectionDefinition(SqlServerDatabase)]
public sealed class DatabaseCollection : ICollectionFixture<SqlServerFixture>
{
    public const string SqlServerDatabase = "SQL Server";
}
