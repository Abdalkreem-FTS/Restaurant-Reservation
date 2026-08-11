using Elastic.Extensions.Logging;
using Elastic.Extensions.Logging.Options;
using Elastic.Ingest.Elasticsearch;
using Microsoft.Extensions.Hosting;
using RestaurantReservation;

// Rooted at the binary rather than the working directory, so appsettings.json is found however the app is started.
var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
{
    Args = args,
    ContentRootPath = AppContext.BaseDirectory
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    Output.Problem("Connection string 'DefaultConnection' is missing. Set it in appsettings.json or as ConnectionStrings__DefaultConnection.");

    return 1;
}

builder.Services.AddRestaurantReservationDb(connectionString);

// The library logs through ILogger and does not care where events end up; this is the host choosing
// Elasticsearch, which is what Kibana reads. Omit the setting and the app just logs to the console.
var elasticsearchUrl = builder.Configuration["Elasticsearch:Url"];

if (!string.IsNullOrWhiteSpace(elasticsearchUrl))
{
    builder.Logging.AddElasticsearch(options =>
    {
        options.ShipTo.NodePoolType = NodePoolType.SingleNode;
        options.ShipTo.NodeUris = [new Uri(elasticsearchUrl)];
        options.DataStream = new DataStreamNameOptions
        {
            Type = "logs",
            DataSet = "restaurant-reservation",
            Namespace = "demo"
        };
        // Silent so an unreachable Elasticsearch costs the logs, not the demo.
        options.BootstrapMethod = BootstrapMethod.Silent;
        options.IncludeScopes = true;
    });
}

var host = builder.Build();

using var cancellation = new CancellationTokenSource();

Console.CancelKeyPress += (_, eventArgs) =>
{
    eventArgs.Cancel = true;
    cancellation.Cancel();
};

int exitCode;

try
{
    var sqlFile = builder.Configuration["sql-file"];

    if (!await DatabaseIsReadyAsync(host.Services, cancellation.Token))
    {
        exitCode = 2;
    }
    else if (!string.IsNullOrWhiteSpace(sqlFile))
    {
        var timeoutSeconds = builder.Configuration.GetValue<int?>("sql-timeout");

        exitCode = await new SqlScriptRunner(host.Services.GetRequiredService<IServiceScopeFactory>())
            .RunAsync(sqlFile, timeoutSeconds is { } seconds ? TimeSpan.FromSeconds(seconds) : null, cancellation.Token);
    }
    else
    {
        if (!string.IsNullOrWhiteSpace(elasticsearchUrl))
        {
            Output.Note($"shipping logs to {elasticsearchUrl}, viewable in Kibana at http://localhost:5601");
        }

        await new Demo(host.Services.GetRequiredService<IServiceScopeFactory>()).RunAsync(cancellation.Token);

        Console.WriteLine();
        Output.Note("done");

        exitCode = 0;
    }
}
catch (OperationCanceledException)
{
    Output.Problem("Cancelled.");

    exitCode = 3;
}
catch (InvalidOperationException exception)
{
    Output.Problem(exception.Message);

    exitCode = 4;
}
finally
{
    // Drains the Elasticsearch channel. This app lives a few seconds, so without disposing the host
    // the last events never leave the process.
    host.Dispose();
}

return exitCode;

static async Task<bool> DatabaseIsReadyAsync(IServiceProvider services, CancellationToken cancellationToken)
{
    using var scope = services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<RestaurantReservationDbContext>();

    try
    {
        if (!await context.Database.CanConnectAsync(cancellationToken))
        {
            Output.Problem("Cannot reach the RestaurantReservationCore database.");
            PrintHint();

            return false;
        }

        var pending = (await context.Database.GetPendingMigrationsAsync(cancellationToken)).ToList();

        if (pending.Count == 0)
        {
            return true;
        }

        Output.Problem($"The database is missing {pending.Count} migration(s): {string.Join(", ", pending)}");
        PrintHint();

        return false;
    }
    catch (Exception exception)
    {
        Output.Problem($"Database check failed: {exception.Message}");
        PrintHint();

        return false;
    }

    static void PrintHint()
    {
        Console.WriteLine();
        Console.WriteLine("  Start SQL Server and apply the schema:");
        Console.WriteLine("    docker compose up -d");
        Console.WriteLine("    dotnet ef database update --project RestaurantReservation.Db");
        Console.WriteLine();
    }
}
