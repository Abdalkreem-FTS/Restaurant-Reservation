using Microsoft.Extensions.Configuration;
using Testcontainers.MsSql;

namespace RestaurantReservation.Tests.Integration;

/// <summary>
/// The one SQL Server container the whole run shares. Each test class puts its own database on it,
/// so the container start is paid for once no matter how many classes there are.
/// </summary>
internal static class SqlServerContainer
{
    private static readonly Lazy<Task<MsSqlContainer>> Started = new(StartAsync);

    public static Task<MsSqlContainer> InstanceAsync() => Started.Value;

    private static async Task<MsSqlContainer> StartAsync()
    {
        var image = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build()["SqlServer:Image"]
            ?? throw new InvalidOperationException("\"SqlServer:Image\" is missing from appsettings.json.");

        var container = new MsSqlBuilder(image).Build();

        await container.StartAsync();

        return container;
    }
}
