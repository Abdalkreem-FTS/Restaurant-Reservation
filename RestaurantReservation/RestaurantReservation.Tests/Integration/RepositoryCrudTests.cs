using RestaurantReservation.Db.Enums;

namespace RestaurantReservation.Tests.Integration;

[Collection(DatabaseCollection.SqlServerDatabase)]
public class RepositoryCrudTests(SqlServerFixture fixture) : DatabaseTest(fixture)
{
    [Fact]
    public async Task AddAsync_WithNewEntity_AssignsTheGeneratedKey()
    {
        var added = await new CustomerRepository(Context).AddAsync(NewCustomer());

        Assert.True(added.CustomerId > 0);
    }

    [Fact]
    public async Task AddAsync_WithNull_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => new CustomerRepository(Context).AddAsync(null!));
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingId_ReturnsTheStoredEntity()
    {
        var repository = new CustomerRepository(Context);
        var added = await repository.AddAsync(NewCustomer());
        Context.ChangeTracker.Clear();

        var stored = await repository.GetByIdAsync(added.CustomerId);

        Assert.NotNull(stored);
        Assert.Equal(added.Email, stored.Email);
    }

    [Fact]
    public async Task GetByIdAsync_WithUnknownId_ReturnsNull()
    {
        Assert.Null(await new CustomerRepository(Context).GetByIdAsync(-1));
    }

    [Fact]
    public async Task GetAllAsync_AfterAddingARow_ReturnsItAlongsideTheExistingOnes()
    {
        var repository = new CustomerRepository(Context);
        var before = await repository.GetAllAsync();

        await repository.AddAsync(NewCustomer());

        Assert.Equal(before.Count + 1, (await repository.GetAllAsync()).Count);
    }

    [Fact]
    public async Task UpdateAsync_WithModifiedEntity_PersistsTheChange()
    {
        var repository = new CustomerRepository(Context);
        var added = await repository.AddAsync(NewCustomer());
        Context.ChangeTracker.Clear();

        var stored = await repository.GetByIdAsync(added.CustomerId);
        stored!.LastName = "Renamed";
        await repository.UpdateAsync(stored);
        Context.ChangeTracker.Clear();

        var updated = await repository.GetByIdAsync(added.CustomerId);

        Assert.Equal("Renamed", updated!.LastName);
    }

    [Fact]
    public async Task UpdateAsync_WithNull_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => new CustomerRepository(Context).UpdateAsync(null!));
    }

    [Fact]
    public async Task DeleteAsync_WithExistingId_RemovesTheEntity()
    {
        var repository = new CustomerRepository(Context);
        var added = await repository.AddAsync(NewCustomer());

        await repository.DeleteAsync(added.CustomerId);
        Context.ChangeTracker.Clear();

        Assert.Null(await repository.GetByIdAsync(added.CustomerId));
    }

    [Fact]
    public async Task DeleteAsync_WithUnknownId_ThrowsKeyNotFoundException()
    {
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => new CustomerRepository(Context).DeleteAsync(-1));

        Assert.Contains("Customer", exception.Message);
    }

    [Fact]
    public async Task DeleteAsync_WithRestaurantThatStillHasChildren_ThrowsDbUpdateException()
    {
        var reservation = await AddReservationAsync();

        await Assert.ThrowsAsync<DbUpdateException>(() => new RestaurantRepository(Context).DeleteAsync(reservation.RestaurantId));
    }

    [Fact]
    public async Task DeleteAsync_WithOrderThatHasItems_CascadesToTheOrderItems()
    {
        var reservation = await AddReservationAsync();
        var orderId = reservation.Orders.Single().OrderId;

        await new OrderRepository(Context).DeleteAsync(orderId);

        Assert.Empty(await Context.OrderItems.Where(item => item.OrderId == orderId).ToListAsync());
    }

    [Fact]
    public async Task Repository_ForEveryEntityType_RoundTripsAddGetDelete()
    {
        var reservation = await AddReservationAsync();
        var order = reservation.Orders.Single();
        var itemId = order.OrderItems.Single().ItemId;

        await AssertRoundTripAsync(new RestaurantRepository(Context), NewRestaurant("Round Trip Bistro"));
        await AssertRoundTripAsync(new CustomerRepository(Context), NewCustomer());
        await AssertRoundTripAsync(new TableRepository(Context), new Table { RestaurantId = reservation.RestaurantId, Capacity = 12 });
        await AssertRoundTripAsync(new EmployeeRepository(Context),
            new Employee
            {
                RestaurantId = reservation.RestaurantId,
                FirstName = "Round",
                LastName = "Trip",
                Position = EmployeePosition.Manager
            });
        await AssertRoundTripAsync(new MenuItemRepository(Context), new MenuItem { RestaurantId = reservation.RestaurantId, Name = "Round Trip Dish", Price = 33.75m });
        await AssertRoundTripAsync(new ReservationRepository(Context),
            new Reservation
            {
                CustomerId = reservation.CustomerId,
                RestaurantId = reservation.RestaurantId,
                TableId = reservation.TableId,
                ReservationDate = new DateTime(2026, 10, 2, 12, 0, 0),
                PartySize = 9
            });
        await AssertRoundTripAsync(new OrderRepository(Context), NewOrder(reservation.ReservationId, order.EmployeeId, 99.95m));
        await AssertRoundTripAsync(new OrderItemRepository(Context), new OrderItem { OrderId = order.OrderId, ItemId = itemId, Quantity = 7 });
    }

    private async Task AssertRoundTripAsync<TEntity>(IRepository<TEntity> repository, TEntity entity)
        where TEntity : class
    {
        var added = await repository.AddAsync(entity);
        var entry = Context.Entry(added);
        var id = (int)entry.Property(entry.Metadata.FindPrimaryKey()!.Properties[0].Name).CurrentValue!;

        Assert.True(id > 0, $"{typeof(TEntity).Name} should be given a generated key");

        Context.ChangeTracker.Clear();
        Assert.NotNull(await repository.GetByIdAsync(id));

        await repository.DeleteAsync(id);

        Context.ChangeTracker.Clear();
        Assert.Null(await repository.GetByIdAsync(id));
    }
}
