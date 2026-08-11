namespace RestaurantReservation.Tests.Integration;

public class QueryTests(DatabaseFixture fixture) : DatabaseTest(fixture), IClassFixture<DatabaseFixture>
{
    private static PageRequest WholePage => new() { Size = PageRequest.MaxSize };

    [Fact]
    public async Task GetTotalRevenueAsync_WithOrdersAtSeveralRestaurants_SumsOnlyTheGivenOne()
    {
        var setup = await AddRestaurantSetupAsync();
        var reservation = await AddReservationAsync(setup);
        await AddOrderAsync(setup, reservation, unitPrice: 10.00m);
        await AddOrderAsync(setup, reservation, unitPrice: 20.00m);

        var elsewhere = await AddRestaurantSetupAsync();
        var elsewhereReservation = await AddReservationAsync(elsewhere);
        await AddOrderAsync(elsewhere, elsewhereReservation, unitPrice: 99.00m);

        var revenue = await Restaurants.GetTotalRevenueAsync(setup.RestaurantId);

        Assert.True(revenue.IsSuccess);
        Assert.Equal(30.00m, revenue.Value);
    }

    [Fact]
    public async Task GetTotalRevenueAsync_WithAnUnknownRestaurant_ReturnsNotFoundRatherThanZero()
    {
        var revenue = await Restaurants.GetTotalRevenueAsync(-1);

        Assert.True(revenue.IsError);
        Assert.Equal("Restaurants.NotFound", revenue.TopError.Code);
        Assert.Equal(ErrorType.NotFound, revenue.TopError.Type);
    }

    [Fact]
    public async Task FindCustomersByPartySizeAsync_WithAThresholdBelowThePartySize_ReturnsTheCustomer()
    {
        var setup = await AddRestaurantSetupAsync(tableCapacity: 8);
        var reservation = await AddReservationAsync(setup, partySize: 6);

        var page = await Customers.FindCustomersByPartySizeAsync(5, WholePage);

        Assert.Contains(page.Items, customer => customer.CustomerId == reservation.CustomerId);
    }

    [Fact]
    public async Task FindCustomersByPartySizeAsync_WithAThresholdEqualToThePartySize_ExcludesTheCustomer()
    {
        var setup = await AddRestaurantSetupAsync(tableCapacity: 8);
        var reservation = await AddReservationAsync(setup, partySize: 6);

        var page = await Customers.FindCustomersByPartySizeAsync(6, WholePage);

        Assert.DoesNotContain(page.Items, customer => customer.CustomerId == reservation.CustomerId);
    }
    
    [Fact]
    public async Task FindCustomersByPartySizeAsync_WithMoreMatchesThanFitOnAPage_StillCountsThemAll()
    {
        var setup = await AddRestaurantSetupAsync(tableCapacity: 8);

        for (var hour = 0; hour < 3; hour++)
        {
            await AddReservationAsync(setup, partySize: 6, at: DefaultSlot.AddHours(hour));
        }

        var expected = await Context.Customers.CountAsync(c => c.Reservations.Any(r => r.PartySize > 5));

        var page = await Customers.FindCustomersByPartySizeAsync(5, new PageRequest { Size = 1 });

        Assert.Single(page.Items);
        Assert.Equal(expected, page.TotalCount);
        Assert.True(page.TotalCount > page.Items.Count);
    }

    [Fact]
    public async Task ListManagersAsync_WithMixedPositions_ReturnsOnlyTheManagers()
    {
        var managers = await AddRestaurantSetupAsync(position: EmployeePosition.Manager);
        var waiters = await AddRestaurantSetupAsync(position: EmployeePosition.StandardWaiter);

        var page = await Employees.ListManagersAsync(WholePage);

        Assert.All(page.Items, employee => Assert.Equal(EmployeePosition.Manager, employee.Position));
        Assert.Contains(page.Items, employee => employee.EmployeeId == managers.Employee.EmployeeId);
        Assert.DoesNotContain(page.Items, employee => employee.EmployeeId == waiters.Employee.EmployeeId);
    }

    [Fact]
    public async Task GetOrderAmountStatisticsAsync_WithSeveralOrders_ReportsTheAggregatesAndThePopulationVariance()
    {
        var setup = await AddRestaurantSetupAsync();
        var reservation = await AddReservationAsync(setup);

        foreach (var amount in new[] { 10.00m, 20.00m, 30.00m, 40.00m })
        {
            await AddOrderAsync(setup, reservation, unitPrice: amount);
        }

        var statistics = await Employees.GetOrderAmountStatisticsAsync(setup.Employee.EmployeeId);

        Assert.Equal(4, statistics.Count);
        Assert.Equal(100.00m, statistics.Sum);
        Assert.Equal(25.00m, statistics.Average);
        Assert.Equal(10.00m, statistics.Min);
        Assert.Equal(40.00m, statistics.Max);
        Assert.Equal(125.00m, statistics.Variance);
    }

    [Fact]
    public async Task GetOrderAmountStatisticsAsync_WithAnEmployeeThatTookNoOrders_ReportsZeros()
    {
        var setup = await AddRestaurantSetupAsync();

        var statistics = await Employees.GetOrderAmountStatisticsAsync(setup.Employee.EmployeeId);

        Assert.Equal(0, statistics.Count);
        Assert.Equal(0m, statistics.Sum);
        Assert.Equal(0m, statistics.Average);
        Assert.Equal(0m, statistics.Min);
        Assert.Equal(0m, statistics.Max);
        Assert.Equal(0m, statistics.Variance);
    }

    [Fact]
    public async Task GetEmployeesWithRestaurantAsync_ReturnsOneViewRowPerEmployeeWithItsRestaurant()
    {
        var setup = await AddRestaurantSetupAsync();

        var page = await Employees.GetEmployeesWithRestaurantAsync(WholePage);

        Assert.Equal(await Context.Employees.CountAsync(), page.TotalCount);

        var row = Assert.Single(page.Items, detail => detail.EmployeeId == setup.Employee.EmployeeId);
        Assert.Equal(setup.Restaurant.Name, row.RestaurantName);
        Assert.Equal(nameof(EmployeePosition.StandardWaiter), row.Position);
    }

    [Fact]
    public async Task GetReservationDetailsAsync_ReturnsOneViewRowPerReservationWithItsCustomerAndRestaurant()
    {
        var setup = await AddRestaurantSetupAsync();
        var reservation = await AddReservationAsync(setup);

        var page = await Reservations.GetReservationDetailsAsync(WholePage);

        Assert.Equal(await Context.Reservations.CountAsync(), page.TotalCount);

        var row = Assert.Single(page.Items, detail => detail.ReservationId == reservation.ReservationId);
        Assert.Equal(setup.Restaurant.Name, row.RestaurantName);
        Assert.Equal(reservation.CustomerId, row.CustomerId);
    }

    [Fact]
    public async Task GetReservationsByCustomerAsync_ReturnsOnlyThatCustomersReservationsMostRecentFirst()
    {
        var setup = await AddRestaurantSetupAsync();
        var earlier = await AddReservationAsync(setup, at: DefaultSlot);
        var later = await AddReservationAsync(setup, at: DefaultSlot.AddHours(2), customerId: earlier.CustomerId);
        var someoneElse = await AddReservationAsync(setup, at: DefaultSlot.AddHours(4));

        var page = await Reservations.GetReservationsByCustomerAsync(earlier.CustomerId, WholePage);

        Assert.Equal(2, page.TotalCount);
        Assert.Equal([later.ReservationId, earlier.ReservationId], page.Items.Select(r => r.ReservationId));
        Assert.DoesNotContain(page.Items, r => r.ReservationId == someoneElse.ReservationId);
        Assert.All(page.Items, r =>
        {
            Assert.NotNull(r.Restaurant);
            Assert.NotNull(r.Table);
        });
    }

    [Fact]
    public async Task ListOrdersAndMenuItemsAsync_ReturnsOnlyThatReservationsOrdersWithTheirMenuItems()
    {
        var setup = await AddRestaurantSetupAsync();
        var reservation = await AddReservationAsync(setup, at: DefaultSlot);
        await AddOrderAsync(setup, reservation);
        await AddOrderAsync(setup, reservation);

        var other = await AddReservationAsync(setup, at: DefaultSlot.AddHours(1));
        await AddOrderAsync(setup, other);

        var page = await Orders.ListOrdersAndMenuItemsAsync(reservation.ReservationId, WholePage);

        Assert.Equal(2, page.TotalCount);
        Assert.All(page.Items, order =>
        {
            Assert.Equal(reservation.ReservationId, order.ReservationId);
            Assert.All(order.OrderItems, item => Assert.NotNull(item.MenuItem));
        });
    }

    [Fact]
    public async Task ListOrderedMenuItemsAsync_WithARepeatedItem_ReturnsEachItemOnce()
    {
        var setup = await AddRestaurantSetupAsync();
        var reservation = await AddReservationAsync(setup);

        await AddOrderAsync(setup, reservation);
        await AddOrderAsync(setup, reservation);

        var second = new MenuItem { RestaurantId = setup.RestaurantId, Name = "Second Dish", Price = 8.00m };
        Context.Add(second);
        await SaveAsync();
        Context.ChangeTracker.Clear();

        var order = NewOrder(setup, reservation);
        order.OrderItems.Single().ItemId = second.ItemId;
        order.RecalculateTotal();
        Context.Add(order);
        await SaveAsync();
        Context.ChangeTracker.Clear();

        var page = await Orders.ListOrderedMenuItemsAsync(reservation.ReservationId, WholePage);

        Assert.Equal(2, page.TotalCount);
        Assert.Equal(
            [setup.MenuItem.ItemId, second.ItemId],
            page.Items.Select(item => item.ItemId).Order());
    }
}
