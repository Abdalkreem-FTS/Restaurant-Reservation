using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace RestaurantReservation.Tests.Model;

[Trait("Category", "Model")]
public class ModelTests
{
    private static readonly IModel EfModel = BuildModel();
    
    [Theory]
    [InlineData(typeof(Table), new[] { nameof(Table.RestaurantId) }, typeof(Restaurant), nameof(Restaurant.Tables), DeleteBehavior.Restrict, new[] { nameof(Restaurant.RestaurantId) })]
    [InlineData(typeof(Employee), new[] { nameof(Employee.RestaurantId) }, typeof(Restaurant), nameof(Restaurant.Employees), DeleteBehavior.Restrict, new[] { nameof(Restaurant.RestaurantId) })]
    [InlineData(typeof(MenuItem), new[] { nameof(MenuItem.RestaurantId) }, typeof(Restaurant), nameof(Restaurant.MenuItems), DeleteBehavior.Restrict, new[] { nameof(Restaurant.RestaurantId) })]
    [InlineData(typeof(Reservation), new[] { nameof(Reservation.CustomerId) }, typeof(Customer), nameof(Customer.Reservations), DeleteBehavior.Restrict, new[] { nameof(Customer.CustomerId) })]
    [InlineData(typeof(Reservation), new[] { nameof(Reservation.RestaurantId) }, typeof(Restaurant), nameof(Restaurant.Reservations), DeleteBehavior.Restrict, new[] { nameof(Restaurant.RestaurantId) })]
    [InlineData(typeof(Reservation), new[] { nameof(Reservation.TableId), nameof(Reservation.RestaurantId), nameof(Reservation.TableCapacity) }, typeof(Table), nameof(Table.Reservations), DeleteBehavior.Restrict, new[] { nameof(Table.TableId), nameof(Table.RestaurantId), nameof(Table.Capacity) })]
    [InlineData(typeof(Order), new[] { nameof(Order.ReservationId), nameof(Order.RestaurantId) }, typeof(Reservation), nameof(Reservation.Orders), DeleteBehavior.Restrict, new[] { nameof(Reservation.ReservationId), nameof(Reservation.RestaurantId) })]
    [InlineData(typeof(Order), new[] { nameof(Order.EmployeeId), nameof(Order.RestaurantId) }, typeof(Employee), nameof(Employee.Orders), DeleteBehavior.Restrict, new[] { nameof(Employee.EmployeeId), nameof(Employee.RestaurantId) })]
    [InlineData(typeof(OrderItem), new[] { nameof(OrderItem.OrderId), nameof(OrderItem.RestaurantId) }, typeof(Order), nameof(Order.OrderItems), DeleteBehavior.Cascade, new[] { nameof(Order.OrderId), nameof(Order.RestaurantId) })]
    [InlineData(typeof(OrderItem), new[] { nameof(OrderItem.ItemId), nameof(OrderItem.RestaurantId) }, typeof(MenuItem), nameof(MenuItem.OrderItems), DeleteBehavior.Restrict, new[] { nameof(MenuItem.ItemId), nameof(MenuItem.RestaurantId) })]
    public void OnModelCreating_ForEachForeignKey_WiresItFromBothEndsOntoTheExpectedPrincipalKey(
        Type dependent,
        string[] properties,
        Type principal,
        string inverseNavigation,
        DeleteBehavior deleteBehavior,
        string[] principalKeyProperties)
    {
        var foreignKey = ForeignKey(dependent, properties);

        Assert.Equal(principal, foreignKey.PrincipalEntityType.ClrType);
        Assert.Equal(principalKeyProperties, foreignKey.PrincipalKey.Properties.Select(property => property.Name));
        Assert.Equal(deleteBehavior, foreignKey.DeleteBehavior);
        Assert.True(foreignKey.IsRequired, $"{dependent.Name}.{string.Join('+', properties)} should be required");

        Assert.NotNull(foreignKey.DependentToPrincipal);
        Assert.False(foreignKey.DependentToPrincipal.IsCollection);

        Assert.NotNull(foreignKey.PrincipalToDependent);
        Assert.Equal(inverseNavigation, foreignKey.PrincipalToDependent.Name);
        Assert.True(foreignKey.PrincipalToDependent.IsCollection);
    }
    
    [Fact]
    public void OnModelCreating_ForEveryEntityImplementingIVersionedEntity_MapsRowVersionAsAConcurrencyToken()
    {
        var versioned = EfModel.GetEntityTypes()
            .Where(entityType => typeof(IVersionedEntity).IsAssignableFrom(entityType.ClrType))
            .ToList();

        Assert.NotEmpty(versioned);
        Assert.All(versioned, entityType =>
        {
            var rowVersion = entityType.FindProperty(nameof(IVersionedEntity.RowVersion))!;

            Assert.True(rowVersion.IsConcurrencyToken, entityType.ClrType.Name);
            Assert.Equal(ValueGenerated.OnAddOrUpdate, rowVersion.ValueGenerated);
            Assert.Equal("rowversion", rowVersion.GetColumnType());
        });
    }
    
    [Fact]
    public void Seed_ForEveryForeignKey_PointsAtRowsThatAreThemselvesSeeded()
    {
        foreach (var entityType in EfModel.GetEntityTypes())
        {
            foreach (var foreignKey in entityType.GetForeignKeys())
            {
                var principalKeys = SeedRows(foreignKey.PrincipalEntityType.ClrType)
                    .Select(row => KeyOf(row, foreignKey.PrincipalKey.Properties))
                    .ToHashSet();

                foreach (var row in SeedRows(entityType.ClrType))
                {
                    Assert.Contains(KeyOf(row, foreignKey.Properties), principalKeys);
                }
            }
        }
    }

    [Theory]
    [InlineData(typeof(IRestaurantRepository))]
    [InlineData(typeof(ICustomerRepository))]
    [InlineData(typeof(ITableRepository))]
    [InlineData(typeof(IEmployeeRepository))]
    [InlineData(typeof(IMenuItemRepository))]
    [InlineData(typeof(IReservationRepository))]
    [InlineData(typeof(IOrderRepository))]
    [InlineData(typeof(IOrderItemRepository))]
    [InlineData(typeof(IUnitOfWork))]
    [InlineData(typeof(RestaurantReservationDbContext))]
    public void AddRestaurantReservationDb_ForEachRegisteredService_ResolvesAnImplementation(Type serviceType)
    {
        using var provider = new ServiceCollection()
            .AddRestaurantReservationDb("Server=(local);Database=NotUsed;Trusted_Connection=True")
            .BuildServiceProvider();
        using var scope = provider.CreateScope();

        Assert.NotNull(scope.ServiceProvider.GetRequiredService(serviceType));
    }

    private static IEntityType EntityType(Type clrType) =>
        EfModel.FindEntityType(clrType) ?? throw new InvalidOperationException($"{clrType.Name} is not part of the model.");

    private static IForeignKey ForeignKey(Type dependent, string[] properties) =>
        EntityType(dependent).GetForeignKeys()
            .Single(key => key.Properties.Select(property => property.Name).SequenceEqual(properties));

    private static List<IDictionary<string, object?>> SeedRows(Type entity) => EntityType(entity).GetSeedData().ToList();

    private static string KeyOf(IDictionary<string, object?> row, IReadOnlyList<IProperty> properties) =>
        string.Join('|', properties.Select(property => row[property.Name]));

    private static IModel BuildModel()
    {
        using var context = new RestaurantReservationDbContext(
            new DbContextOptionsBuilder<RestaurantReservationDbContext>().UseSqlServer().Options);

        return context.GetService<IDesignTimeModel>().Model;
    }
}
