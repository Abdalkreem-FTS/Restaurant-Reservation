using RestaurantReservation.Db.Data;
using RestaurantReservation.Db.Entities;
using RestaurantReservation.Db.Entities.Views;

namespace RestaurantReservation.Db;

public class RestaurantReservationDbContext(DbContextOptions<RestaurantReservationDbContext> options) : DbContext(options)
{
    // Tables
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Restaurant> Restaurants => Set<Restaurant>();
    public DbSet<Table> Tables => Set<Table>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<MenuItem> MenuItems => Set<MenuItem>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    // Views
    public DbSet<ReservationDetail> ReservationDetails => Set<ReservationDetail>();
    public DbSet<EmployeeRestaurantDetail> EmployeeDetails => Set<EmployeeRestaurantDetail>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RestaurantReservationDbContext).Assembly);

        modelBuilder.Seed();
    }
    
    // Functions
    [DbFunction("fn_CalculateRestaurantRevenue", "dbo")]
    public static decimal CalculateRestaurantRevenue(int restaurantId)
    {
        throw new NotSupportedException("CalculateRestaurantRevenue maps to a SQL function and can only be used within an EF Core LINQ query.");
    }
}