using Microsoft.Extensions.Logging;
using RestaurantReservation;

var options = DemoOptions.Parse(args);

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

var connectionString = configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    Console.Error.WriteLine(
        "Connection string 'DefaultConnection' was not found. Set it in appsettings.json or via the " +
        "ConnectionStrings__DefaultConnection environment variable.");

    return DemoRunner.DatabaseUnavailable;
}

var contextOptions = new DbContextOptionsBuilder<RestaurantReservationDbContext>().UseSqlServer(connectionString);

if (options.ShowSql)
{
    contextOptions.LogTo(Console.WriteLine, [DbLoggerCategory.Database.Command.Name], LogLevel.Information);
}

using var cancellation = new CancellationTokenSource();

Console.CancelKeyPress += (_, eventArgs) =>
{
    eventArgs.Cancel = true;
    cancellation.Cancel();
};

return await new DemoRunner(NewContext).RunAsync(options, cancellation.Token);


RestaurantReservationDbContext NewContext() => new(contextOptions.Options);
