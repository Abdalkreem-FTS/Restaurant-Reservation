using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using RestaurantReservation.Db.Entities.Views;
using RestaurantReservation.Db.Enums;

namespace RestaurantReservation.Tests.Model;

[Trait("Category", "Model")]
public class ModelTests
{
    private static readonly IModel EfModel = BuildModel();

    [Theory]
    [InlineData(typeof(Restaurant), "Restaurants", nameof(Restaurant.RestaurantId))]
    [InlineData(typeof(Customer), "Customers", nameof(Customer.CustomerId))]
    [InlineData(typeof(Table), "Tables", nameof(Table.TableId))]
    [InlineData(typeof(Employee), "Employees", nameof(Employee.EmployeeId))]
    [InlineData(typeof(MenuItem), "MenuItems", nameof(MenuItem.ItemId))]
    [InlineData(typeof(Reservation), "Reservations", nameof(Reservation.ReservationId))]
    [InlineData(typeof(Order), "Orders", nameof(Order.OrderId))]
    [InlineData(typeof(OrderItem), "OrderItems", nameof(OrderItem.OrderItemId))]
    public void OnModelCreating_ForEachEntity_MapsItToItsTableWithAGeneratedKey(Type entity, string table, string keyProperty)
    {
        var primaryKey = EntityType(entity).FindPrimaryKey();

        Assert.Equal(table, EntityType(entity).GetTableName());
        Assert.NotNull(primaryKey);
        Assert.Equal(keyProperty, Assert.Single(primaryKey.Properties).Name);
        Assert.Equal(ValueGenerated.OnAdd, Property(entity, keyProperty).ValueGenerated);
    }

    [Theory]
    [InlineData(typeof(Restaurant), nameof(Restaurant.Name), 100)]
    [InlineData(typeof(Restaurant), nameof(Restaurant.Address), 200)]
    [InlineData(typeof(Restaurant), nameof(Restaurant.PhoneNumber), 20)]
    [InlineData(typeof(Restaurant), nameof(Restaurant.OpeningHours), 100)]
    [InlineData(typeof(Customer), nameof(Customer.FirstName), 50)]
    [InlineData(typeof(Customer), nameof(Customer.LastName), 50)]
    [InlineData(typeof(Customer), nameof(Customer.Email), 254)]
    [InlineData(typeof(Customer), nameof(Customer.PhoneNumber), 20)]
    [InlineData(typeof(Employee), nameof(Employee.FirstName), 50)]
    [InlineData(typeof(Employee), nameof(Employee.LastName), 50)]
    [InlineData(typeof(Employee), nameof(Employee.Position), 20)]
    [InlineData(typeof(MenuItem), nameof(MenuItem.Name), 100)]
    [InlineData(typeof(MenuItem), nameof(MenuItem.Description), 500)]
    public void OnModelCreating_ForEachStringColumn_AppliesTheExpectedMaximumLength(Type entity, string property, int maxLength)
    {
        Assert.Equal(maxLength, Property(entity, property).GetMaxLength());
    }

    [Theory]
    [InlineData(typeof(Restaurant), nameof(Restaurant.Name), false)]
    [InlineData(typeof(Restaurant), nameof(Restaurant.Address), false)]
    [InlineData(typeof(Restaurant), nameof(Restaurant.PhoneNumber), false)]
    [InlineData(typeof(Restaurant), nameof(Restaurant.OpeningHours), false)]
    [InlineData(typeof(Customer), nameof(Customer.FirstName), false)]
    [InlineData(typeof(Customer), nameof(Customer.LastName), false)]
    [InlineData(typeof(Customer), nameof(Customer.Email), false)]
    [InlineData(typeof(Customer), nameof(Customer.PhoneNumber), false)]
    [InlineData(typeof(Employee), nameof(Employee.FirstName), false)]
    [InlineData(typeof(Employee), nameof(Employee.LastName), false)]
    [InlineData(typeof(Employee), nameof(Employee.Position), false)]
    [InlineData(typeof(MenuItem), nameof(MenuItem.Name), false)]
    [InlineData(typeof(MenuItem), nameof(MenuItem.Description), true)]
    [InlineData(typeof(Table), nameof(Table.Capacity), false)]
    [InlineData(typeof(Reservation), nameof(Reservation.ReservationDate), false)]
    [InlineData(typeof(Reservation), nameof(Reservation.PartySize), false)]
    [InlineData(typeof(Order), nameof(Order.OrderDate), false)]
    [InlineData(typeof(OrderItem), nameof(OrderItem.Quantity), false)]
    public void OnModelCreating_ForEachColumn_MakesItNullableOnlyWhereConfigured(Type entity, string property, bool isNullable)
    {
        Assert.Equal(isNullable, Property(entity, property).IsNullable);
    }

    [Theory]
    [InlineData(typeof(MenuItem), nameof(MenuItem.Price))]
    [InlineData(typeof(Order), nameof(Order.TotalAmount))]
    public void OnModelCreating_ForEachMoneyColumn_AppliesTwoDecimalPlaces(Type entity, string property)
    {
        var money = Property(entity, property);

        Assert.Equal(18, money.GetPrecision());
        Assert.Equal(2, money.GetScale());
    }

    [Fact]
    public void OnModelCreating_ForEmployeePosition_StoresTheEnumAsAString()
    {
        var position = Property(typeof(Employee), nameof(Employee.Position));

        Assert.Equal(typeof(EmployeePosition), position.ClrType);
        Assert.Equal(typeof(string), position.GetProviderClrType());
    }

    [Fact]
    public void OnModelCreating_ForCustomerEmail_AddsAUniqueIndex()
    {
        var index = Assert.Single(EntityType(typeof(Customer)).GetIndexes());

        Assert.True(index.IsUnique);
        Assert.Equal(nameof(Customer.Email), Assert.Single(index.Properties).Name);
    }

    [Theory]
    [InlineData(typeof(Table), nameof(Table.RestaurantId), typeof(Restaurant), nameof(Restaurant.Tables), DeleteBehavior.Restrict)]
    [InlineData(typeof(Employee), nameof(Employee.RestaurantId), typeof(Restaurant), nameof(Restaurant.Employees), DeleteBehavior.Restrict)]
    [InlineData(typeof(MenuItem), nameof(MenuItem.RestaurantId), typeof(Restaurant), nameof(Restaurant.MenuItems), DeleteBehavior.Restrict)]
    [InlineData(typeof(Reservation), nameof(Reservation.CustomerId), typeof(Customer), nameof(Customer.Reservations), DeleteBehavior.Restrict)]
    [InlineData(typeof(Reservation), nameof(Reservation.RestaurantId), typeof(Restaurant), nameof(Restaurant.Reservations), DeleteBehavior.Restrict)]
    [InlineData(typeof(Reservation), nameof(Reservation.TableId), typeof(Table), nameof(Table.Reservations), DeleteBehavior.Restrict)]
    [InlineData(typeof(Order), nameof(Order.ReservationId), typeof(Reservation), nameof(Reservation.Orders), DeleteBehavior.Restrict)]
    [InlineData(typeof(Order), nameof(Order.EmployeeId), typeof(Employee), nameof(Employee.Orders), DeleteBehavior.Restrict)]
    [InlineData(typeof(OrderItem), nameof(OrderItem.ItemId), typeof(MenuItem), nameof(MenuItem.OrderItems), DeleteBehavior.Restrict)]
    [InlineData(typeof(OrderItem), nameof(OrderItem.OrderId), typeof(Order), nameof(Order.OrderItems), DeleteBehavior.Cascade)]
    public void OnModelCreating_ForEachForeignKey_MakesItRequiredAndWiredFromBothEnds(
        Type dependent,
        string foreignKeyProperty,
        Type principal,
        string inverseNavigation,
        DeleteBehavior deleteBehavior)
    {
        var foreignKey = ForeignKey(dependent, foreignKeyProperty);

        Assert.Equal(principal, foreignKey.PrincipalEntityType.ClrType);
        Assert.Equal(deleteBehavior, foreignKey.DeleteBehavior);
        Assert.True(foreignKey.IsRequired, $"{dependent.Name}.{foreignKeyProperty} should be required");

        Assert.NotNull(foreignKey.DependentToPrincipal);
        Assert.False(foreignKey.DependentToPrincipal.IsCollection);

        Assert.NotNull(foreignKey.PrincipalToDependent);
        Assert.Equal(inverseNavigation, foreignKey.PrincipalToDependent.Name);
        Assert.True(foreignKey.PrincipalToDependent.IsCollection);
    }

    [Theory]
    [InlineData(typeof(ReservationDetail), "vw_ReservationDetails")]
    [InlineData(typeof(EmployeeRestaurantDetail), "vw_EmployeeDetails")]
    public void OnModelCreating_ForEachViewType_MapsItKeylessToItsView(Type entity, string viewName)
    {
        var entityType = EntityType(entity);

        Assert.Equal(viewName, entityType.GetViewName());
        Assert.Null(entityType.GetTableName());
        Assert.Null(entityType.FindPrimaryKey());
        Assert.Empty(entityType.GetForeignKeys());
    }

    [Theory]
    [InlineData(typeof(Table), nameof(Table.RestaurantId), typeof(Restaurant), nameof(Restaurant.RestaurantId))]
    [InlineData(typeof(Employee), nameof(Employee.RestaurantId), typeof(Restaurant), nameof(Restaurant.RestaurantId))]
    [InlineData(typeof(MenuItem), nameof(MenuItem.RestaurantId), typeof(Restaurant), nameof(Restaurant.RestaurantId))]
    [InlineData(typeof(Reservation), nameof(Reservation.CustomerId), typeof(Customer), nameof(Customer.CustomerId))]
    [InlineData(typeof(Reservation), nameof(Reservation.RestaurantId), typeof(Restaurant), nameof(Restaurant.RestaurantId))]
    [InlineData(typeof(Reservation), nameof(Reservation.TableId), typeof(Table), nameof(Table.TableId))]
    [InlineData(typeof(Order), nameof(Order.ReservationId), typeof(Reservation), nameof(Reservation.ReservationId))]
    [InlineData(typeof(Order), nameof(Order.EmployeeId), typeof(Employee), nameof(Employee.EmployeeId))]
    [InlineData(typeof(OrderItem), nameof(OrderItem.OrderId), typeof(Order), nameof(Order.OrderId))]
    [InlineData(typeof(OrderItem), nameof(OrderItem.ItemId), typeof(MenuItem), nameof(MenuItem.ItemId))]
    public void Seed_ForEachSeededForeignKey_PointsAtASeededRow(
        Type dependent,
        string foreignKeyProperty,
        Type principal,
        string principalKeyProperty)
    {
        var principalKeys = SeedRows(principal).Select(row => row[principalKeyProperty]).ToHashSet();
        var references = SeedRows(dependent).Select(row => row[foreignKeyProperty]);

        Assert.All(references, reference => Assert.Contains(reference, principalKeys));
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
    public void AddRestaurantReservationDb_ForEachRepositoryInterface_ResolvesAnImplementation(Type serviceType)
    {
        using var provider = new ServiceCollection()
            .AddRestaurantReservationDb("Server=(local);Database=NotUsed;Trusted_Connection=True")
            .BuildServiceProvider();
        using var scope = provider.CreateScope();

        Assert.NotNull(scope.ServiceProvider.GetRequiredService(serviceType));
    }

    private static IEntityType EntityType(Type clrType) => EfModel.FindEntityType(clrType) ?? throw new InvalidOperationException($"{clrType.Name} is not part of the model.");

    private static IProperty Property(Type clrType, string name) => EntityType(clrType).FindProperty(name) ?? throw new InvalidOperationException($"{clrType.Name}.{name} is not mapped.");

    private static IForeignKey ForeignKey(Type dependent, string foreignKeyProperty) => EntityType(dependent).GetForeignKeys().Single(key => key.Properties.Any(p => p.Name == foreignKeyProperty));

    private static List<IDictionary<string, object?>> SeedRows(Type entity) => EntityType(entity).GetSeedData().ToList();

    private static IModel BuildModel()
    {
        using var context = new RestaurantReservationDbContext(new DbContextOptionsBuilder<RestaurantReservationDbContext>().UseSqlServer().Options);

        return context.GetService<IDesignTimeModel>().Model;
    }
}
