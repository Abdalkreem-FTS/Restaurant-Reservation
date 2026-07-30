using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using RestaurantReservation.Db.Abstractions;
using RestaurantReservation.Db.Logging;
using RestaurantReservation.Db.Repositories;

namespace RestaurantReservation.Db;

public static class DependencyInjection
{
    private static readonly TimeSpan DefaultSlowCommandThreshold = TimeSpan.FromMilliseconds(500);

    /// <summary>
    /// Registers the context, the unit of work and every repository, with logging wired to whatever
    /// provider the host installed. The host owns that choice entirely; nothing here depends on where
    /// the events end up.
    /// </summary>
    /// <param name="services">The collection the registrations are added to.</param>
    /// <param name="connectionString">Connection string for the SQL Server database to use.</param>
    /// <param name="slowCommandThreshold">
    /// How long a successful command may take before it is logged as slow. Defaults to 500ms.
    /// </param>
    public static IServiceCollection AddRestaurantReservationDb(
        this IServiceCollection services,
        string connectionString,
        TimeSpan? slowCommandThreshold = null)
    {
        services.AddLogging();

        services.AddDbContext<RestaurantReservationDbContext>((serviceProvider, options) =>
        {
            options.UseSqlServer(connectionString);

            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

            options.AddInterceptors(
                new CommandLoggingInterceptor(loggerFactory.CreateLogger<CommandLoggingInterceptor>(),
                    slowCommandThreshold ?? DefaultSlowCommandThreshold));
            
            options.ConfigureWarnings(warnings => warnings.Log(
                (RelationalEventId.CommandExecuted, LogLevel.Debug),
                (RelationalEventId.CommandError, LogLevel.Debug)));
        });

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IRestaurantRepository, RestaurantRepository>();
        services.AddScoped<ITableRepository, TableRepository>();
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IMenuItemRepository, MenuItemRepository>();
        services.AddScoped<IReservationRepository, ReservationRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IOrderItemRepository, OrderItemRepository>();

        return services;
    }
}
