namespace RestaurantReservation.Tests.Integration;

public class RepositoryTests(DatabaseFixture fixture) : DatabaseTest(fixture), IClassFixture<DatabaseFixture>
{
    [Fact]
    public async Task AddAsync_WithNull_ReturnsAValidationErrorInsteadOfThrowing()
    {
        var result = await Customers.AddAsync(null);

        Assert.True(result.IsError);
        Assert.Equal("Repository.NullEntity", result.TopError.Code);
        Assert.Equal(ErrorType.Validation, result.TopError.Type);
    }

    [Fact]
    public void Update_WithNull_ReturnsAValidationErrorInsteadOfThrowing()
    {
        var result = Customers.Update(null);

        Assert.True(result.IsError);
        Assert.Equal("Repository.NullEntity", result.TopError.Code);
    }

    [Fact]
    public async Task AddAsync_BeforeTheUnitOfWorkSaves_LeavesTheRowUnwritten()
    {
        var customer = NewCustomer();

        Assert.True((await Customers.AddAsync(customer)).IsSuccess);

        await using var other = NewContext();

        Assert.Equal(0, await other.Customers.CountAsync(c => c.Email == customer.Email));
    }

    [Fact]
    public async Task SaveChangesAsync_AfterAdding_WritesTheRowAndAssignsItsKey()
    {
        var customer = NewCustomer();

        Assert.True((await Customers.AddAsync(customer)).IsSuccess);
        Assert.Equal(1, await SaveAsync());
        Assert.True(customer.CustomerId > 0);

        await using var other = NewContext();

        Assert.Equal(1, await other.Customers.CountAsync(c => c.Email == customer.Email));
    }

    [Fact]
    public async Task GetByIdAsync_WithAnExistingId_ReturnsTheStoredEntity()
    {
        var customer = NewCustomer();
        await Customers.AddAsync(customer);
        await SaveAsync();
        Context.ChangeTracker.Clear();

        var result = await Customers.GetByIdAsync(customer.CustomerId);

        Assert.True(result.IsSuccess);
        Assert.Equal(customer.Email, result.Value.Email);
    }

    [Fact]
    public async Task GetByIdAsync_WithAnUnknownId_ReturnsNotFoundInsteadOfNull()
    {
        var result = await Customers.GetByIdAsync(-1);

        Assert.True(result.IsError);
        Assert.Equal("Repository.NotFound", result.TopError.Code);
        Assert.Equal(ErrorType.NotFound, result.TopError.Type);
    }

    [Fact]
    public async Task DeleteAsync_WithAnUnknownId_ReturnsNotFoundInsteadOfThrowing()
    {
        var result = await Customers.DeleteAsync(-1);

        Assert.True(result.IsError);
        Assert.Equal("Repository.NotFound", result.TopError.Code);
    }

    [Fact]
    public async Task DeleteAsync_ThenSaving_RemovesTheRow()
    {
        var customer = NewCustomer();
        await Customers.AddAsync(customer);
        await SaveAsync();
        Context.ChangeTracker.Clear();

        Assert.True((await Customers.DeleteAsync(customer.CustomerId)).IsSuccess);
        await SaveAsync();
        Context.ChangeTracker.Clear();

        Assert.True((await Customers.GetByIdAsync(customer.CustomerId)).IsError);
    }

    [Fact]
    public async Task Update_ThenSaving_PersistsTheChange()
    {
        var customer = NewCustomer();
        await Customers.AddAsync(customer);
        await SaveAsync();
        Context.ChangeTracker.Clear();

        var stored = (await Customers.GetByIdAsync(customer.CustomerId)).Value;
        stored.LastName = "Renamed";

        Assert.True(Customers.Update(stored).IsSuccess);
        await SaveAsync();
        Context.ChangeTracker.Clear();

        Assert.Equal("Renamed", (await Customers.GetByIdAsync(customer.CustomerId)).Value.LastName);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsOnlyTheRequestedPageButCountsEveryRow()
    {
        var page = await Customers.GetAllAsync(new PageRequest { Number = 1, Size = 2 });

        Assert.Equal(2, page.Items.Count);
        Assert.Equal(await Context.Customers.CountAsync(), page.TotalCount);
        Assert.Equal(1, page.PageNumber);
        Assert.False(page.HasPrevious);
        Assert.True(page.HasNext);
    }

    [Fact]
    public async Task GetAllAsync_WithASizeAboveTheMaximum_PagesAtTheMaximumInstead()
    {
        var page = await Customers.GetAllAsync(new PageRequest { Size = 5_000 });

        Assert.Equal(PageRequest.MaxSize, page.PageSize);
    }

    [Fact]
    public async Task FindAsync_WithAPredicate_FiltersBeforePaginating()
    {
        var surname = $"Filter{Guid.NewGuid():N}";

        for (var i = 0; i < 3; i++)
        {
            var customer = NewCustomer();
            customer.LastName = surname;
            await Customers.AddAsync(customer);
        }

        await SaveAsync();
        Context.ChangeTracker.Clear();

        var page = await Customers.FindAsync(customer => customer.LastName == surname, new PageRequest { Size = 2 });

        Assert.Equal(2, page.Items.Count);
        Assert.Equal(3, page.TotalCount);
        Assert.All(page.Items, customer => Assert.Equal(surname, customer.LastName));
    }

    [Fact]
    public async Task Repositories_ForAnEntityWithRelations_RoundTripAddSaveGetDelete()
    {
        var setup = await AddRestaurantSetupAsync();
        var reservation = await AddReservationAsync(setup);
        var order = await AddOrderAsync(setup, reservation);

        Assert.True((await Reservations.GetByIdAsync(reservation.ReservationId)).IsSuccess);
        Assert.True((await Orders.GetByIdAsync(order.OrderId)).IsSuccess);
        Assert.True((await Tables.GetByIdAsync(setup.Table.TableId)).IsSuccess);
        Assert.True((await Employees.GetByIdAsync(setup.Employee.EmployeeId)).IsSuccess);
        Assert.True((await MenuItems.GetByIdAsync(setup.MenuItem.ItemId)).IsSuccess);
        Assert.True((await Restaurants.GetByIdAsync(setup.RestaurantId)).IsSuccess);

        var item = await Context.OrderItems.SingleAsync(orderItem => orderItem.OrderId == order.OrderId);
        Assert.True((await OrderItems.GetByIdAsync(item.OrderItemId)).IsSuccess);

        Assert.True((await Orders.DeleteAsync(order.OrderId)).IsSuccess);
        await SaveAsync();
        Context.ChangeTracker.Clear();

        Assert.True((await Orders.GetByIdAsync(order.OrderId)).IsError);
    }
}
