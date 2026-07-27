using RestaurantReservation.Db.Abstractions;
using RestaurantReservation.Db.Repositories;

namespace RestaurantReservation.Db;

public static class DependencyInjection
{
    public static IServiceCollection AddRestaurantReservationDb(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<RestaurantReservationDbContext>(options => options.UseSqlServer(connectionString));

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
