namespace RestaurantReservation.Tests.Integration;

public class ConstraintTests(DatabaseFixture fixture) : DatabaseTest(fixture), IClassFixture<DatabaseFixture>
{
    [Fact]
    public async Task SaveChanges_WithAPartySizeOfZero_ReportsPartySizeIsNotPositive()
    {
        var setup = await AddRestaurantSetupAsync();

        var reservation = NewReservation(setup, partySize: 0);
        reservation.Customer = NewCustomer();
        Context.Add(reservation);

        AssertFailedWith("Reservations.PartySizeIsNotPositive", ErrorType.Validation, await TrySaveAsync());
    }

    [Fact]
    public async Task SaveChanges_WithAPartySizeAboveTheTableCapacity_ReportsPartySizeExceedsTableCapacity()
    {
        var setup = await AddRestaurantSetupAsync(tableCapacity: 4);

        var reservation = NewReservation(setup, partySize: 6);
        reservation.Customer = NewCustomer();
        Context.Add(reservation);

        AssertFailedWith("Reservations.PartySizeExceedsTableCapacity", ErrorType.Validation, await TrySaveAsync());
    }

    [Fact]
    public async Task SaveChanges_WithAReservationOffTheHour_ReportsReservationDateNotOnTheHour()
    {
        var setup = await AddRestaurantSetupAsync();

        var reservation = NewReservation(setup, at: new DateTime(2026, 10, 1, 19, 30, 0));
        reservation.Customer = NewCustomer();
        Context.Add(reservation);

        AssertFailedWith("Reservations.ReservationDateNotOnTheHour", ErrorType.Validation, await TrySaveAsync());
    }
    
    [Fact]
    public async Task SaveChanges_WithATableCapacityThatDoesNotMatchTheTable_ReportsTableNotAvailableAtRestaurant()
    {
        var setup = await AddRestaurantSetupAsync(tableCapacity: 4);

        var reservation = NewReservation(setup, partySize: 1);
        reservation.TableCapacity = 99;
        reservation.Customer = NewCustomer();
        Context.Add(reservation);

        AssertFailedWith("Reservations.TableNotAvailableAtRestaurant", ErrorType.Validation, await TrySaveAsync());
    }

    [Fact]
    public async Task SaveChanges_WithTheSameTableBookedTwiceAtTheSameHour_ReportsTableAlreadyBooked()
    {
        var setup = await AddRestaurantSetupAsync();
        await AddReservationAsync(setup, at: DefaultSlot);

        var clash = NewReservation(setup, at: DefaultSlot);
        clash.Customer = NewCustomer();
        Context.Add(clash);

        AssertFailedWith("Reservations.TableAlreadyBooked", ErrorType.Conflict, await TrySaveAsync());
    }

    [Fact]
    public async Task SaveChanges_WithAQuantityOfZero_ReportsQuantityIsNotPositive()
    {
        var setup = await AddRestaurantSetupAsync();
        var reservation = await AddReservationAsync(setup);

        Context.Add(NewOrder(setup, reservation, quantity: 0));

        AssertFailedWith("OrderItems.QuantityIsNotPositive", ErrorType.Validation, await TrySaveAsync());
    }

    [Fact]
    public async Task SaveChanges_WithANegativeUnitPrice_ReportsUnitPriceIsNegative()
    {
        var setup = await AddRestaurantSetupAsync();
        var reservation = await AddReservationAsync(setup);

        Context.Add(NewOrder(setup, reservation, unitPrice: -1.00m));

        AssertFailedWith("OrderItems.UnitPriceIsNegative", ErrorType.Validation, await TrySaveAsync());
    }

    [Fact]
    public async Task SaveChanges_WithANegativeMenuItemPrice_ReportsPriceIsNegative()
    {
        var setup = await AddRestaurantSetupAsync();

        Context.Add(new MenuItem
        {
            RestaurantId = setup.RestaurantId,
            Name = "Underpriced",
            Price = -1.00m
        });

        AssertFailedWith("MenuItems.PriceIsNegative", ErrorType.Validation, await TrySaveAsync());
    }

    [Fact]
    public async Task SaveChanges_WithATableCapacityOfZero_ReportsCapacityIsNotPositive()
    {
        var setup = await AddRestaurantSetupAsync();

        Context.Add(new Table { RestaurantId = setup.RestaurantId, Capacity = 0 });

        AssertFailedWith("Tables.CapacityIsNotPositive", ErrorType.Validation, await TrySaveAsync());
    }

    [Fact]
    public async Task SaveChanges_WithAnEmailThatIsAlreadyTaken_ReportsDuplicateEmail()
    {
        var existing = NewCustomer();
        Context.Add(existing);
        await SaveAsync();
        Context.ChangeTracker.Clear();

        var duplicate = NewCustomer();
        duplicate.Email = existing.Email;
        Context.Add(duplicate);

        AssertFailedWith("Customers.DuplicateEmail", ErrorType.Conflict, await TrySaveAsync());
    }

    [Fact]
    public async Task SaveChanges_WithAnEmployeeFromAnotherRestaurant_ReportsEmployeeNotAtRestaurant()
    {
        var setup = await AddRestaurantSetupAsync();
        var elsewhere = await AddRestaurantSetupAsync();
        var reservation = await AddReservationAsync(setup);

        var order = NewOrder(setup, reservation);
        order.EmployeeId = elsewhere.Employee.EmployeeId;
        Context.Add(order);

        AssertFailedWith("Orders.EmployeeNotAtRestaurant", ErrorType.Validation, await TrySaveAsync());
    }

    [Fact]
    public async Task SaveChanges_WithAMenuItemFromAnotherRestaurant_ReportsMenuItemNotAtRestaurant()
    {
        var setup = await AddRestaurantSetupAsync();
        var elsewhere = await AddRestaurantSetupAsync();
        var reservation = await AddReservationAsync(setup);
        var order = await AddOrderAsync(setup, reservation);

        Context.Add(new OrderItem
        {
            OrderId = order.OrderId,
            RestaurantId = setup.RestaurantId,
            ItemId = elsewhere.MenuItem.ItemId,
            Quantity = 1,
            UnitPrice = 5.00m
        });

        AssertFailedWith("OrderItems.MenuItemNotAtRestaurant", ErrorType.Validation, await TrySaveAsync());
    }

    [Fact]
    public async Task SaveChanges_DeletingARestaurantThatStillHasTables_ReportsHasTables()
    {
        var restaurant = NewRestaurant();
        Context.Add(restaurant);
        Context.Add(new Table { Restaurant = restaurant, Capacity = 4 });
        await SaveAsync();
        Context.ChangeTracker.Clear();

        Assert.True((await Restaurants.DeleteAsync(restaurant.RestaurantId)).IsSuccess);

        AssertFailedWith("Restaurants.HasTables", ErrorType.Conflict, await TrySaveAsync());
    }

    [Fact]
    public async Task SaveChanges_DeletingACustomerThatStillHasReservations_ReportsHasReservations()
    {
        var setup = await AddRestaurantSetupAsync();
        var reservation = await AddReservationAsync(setup);

        Assert.True((await Customers.DeleteAsync(reservation.CustomerId)).IsSuccess);

        AssertFailedWith("Customers.HasReservations", ErrorType.Conflict, await TrySaveAsync());
    }
    
    [Fact]
    public async Task SaveChanges_DeletingAnOrder_CascadesToItsOrderItems()
    {
        var setup = await AddRestaurantSetupAsync();
        var reservation = await AddReservationAsync(setup);
        var order = await AddOrderAsync(setup, reservation);

        Assert.True((await Orders.DeleteAsync(order.OrderId)).IsSuccess);
        await SaveAsync();

        Assert.Empty(await Context.OrderItems.Where(item => item.OrderId == order.OrderId).ToListAsync());
    }

    [Fact]
    public async Task SaveChanges_WhenAnotherContextChangedTheRowFirst_ReportsConcurrencyConflict()
    {
        var setup = await AddRestaurantSetupAsync(tableCapacity: 8);
        var reservation = await AddReservationAsync(setup);

        var mine = await Context.Reservations.SingleAsync(r => r.ReservationId == reservation.ReservationId);

        await using (var other = NewContext())
        {
            var theirs = await other.Reservations.SingleAsync(r => r.ReservationId == reservation.ReservationId);
            theirs.PartySize = 3;
            await other.SaveChangesAsync();
        }

        mine.PartySize = 4;

        AssertFailedWith("Db.ConcurrencyConflict", ErrorType.Conflict, await TrySaveAsync());
    }

    private static void AssertFailedWith(string code, ErrorType type, Result<int> result)
    {
        Assert.True(result.IsError, "the save was expected to fail but succeeded");
        Assert.Equal(code, result.TopError.Code);
        Assert.Equal(type, result.TopError.Type);
    }
}
