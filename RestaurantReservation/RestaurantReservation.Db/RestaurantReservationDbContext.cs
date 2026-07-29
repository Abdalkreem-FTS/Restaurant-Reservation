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

        ConfigureConcurrencyTokens(modelBuilder);

        modelBuilder.Seed();
    }

    /// <summary>
    /// Maps every <see cref="IVersionedEntity" />'s token to a SQL Server <c>rowversion</c> column.
    /// Applied by convention rather than repeated across the per-entity configurations so a newly
    /// added entity cannot silently opt out of lost-update detection.
    /// </summary>
    private static void ConfigureConcurrencyTokens(ModelBuilder modelBuilder)
    {
        var versionedEntities = modelBuilder.Model
            .GetEntityTypes()
            .Where(entityType => typeof(IVersionedEntity).IsAssignableFrom(entityType.ClrType));

        foreach (var entityType in versionedEntities)
        {
            modelBuilder.Entity(entityType.ClrType)
                .Property(nameof(IVersionedEntity.RowVersion))
                .IsRowVersion();
        }
    }
    
    // Functions
    [DbFunction("fn_CalculateRestaurantRevenue", "dbo")]
    public static decimal CalculateRestaurantRevenue(int restaurantId)
    {
        throw new NotSupportedException("CalculateRestaurantRevenue maps to a SQL function and can only be used within an EF Core LINQ query.");
    }
}
