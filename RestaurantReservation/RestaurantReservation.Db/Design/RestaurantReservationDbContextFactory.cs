namespace RestaurantReservation.Db.Design;

/// <summary>
/// Enables the EF Core CLI (<c>dotnet ef migrations add</c>, <c>database update</c>, ...) to build the
/// <see cref="RestaurantReservationDbContext" /> at design time without a separate startup application.
/// The connection string is read from <c>appsettings.json</c> and can be overridden via the
/// <c>ConnectionStrings__DefaultConnection</c> environment variable.
/// </summary>
public sealed class RestaurantReservationDbContextFactory : IDesignTimeDbContextFactory<RestaurantReservationDbContext>
{
    public RestaurantReservationDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
                               ?? throw new InvalidOperationException(
                                   "Connection string 'DefaultConnection' was not found. " +
                                   "Set it in appsettings.json or via the ConnectionStrings__DefaultConnection environment variable.");

        var optionsBuilder = new DbContextOptionsBuilder<RestaurantReservationDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new RestaurantReservationDbContext(optionsBuilder.Options);
    }
}
