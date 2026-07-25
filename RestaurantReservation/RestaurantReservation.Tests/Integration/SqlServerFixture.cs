using Microsoft.Data.SqlClient;
using Testcontainers.MsSql;

namespace RestaurantReservation.Tests.Integration;

public sealed class SqlServerFixture : IAsyncLifetime
{
    private readonly MsSqlContainer _container = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2025-latest").Build();

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
