using RestaurantReservation.Db.Enums;

namespace RestaurantReservation.Tests.Integration;

[Collection(DatabaseCollection.SqlServerDatabase)]
public class RepositoryQueryTests(SqlServerFixture fixture) : DatabaseTest(fixture)
{
    [Fact]
    public async Task FindCustomersByPartySizeAsync_WithThresholdBelowThePartySize_ReturnsTheCustomer()
    {
        var reservation = await AddReservationAsync(partySize: 8);

        var customers = await new CustomerRepository(Context).FindCustomersByPartySizeAsync(7);

        Assert.Contains(customers, customer => customer.CustomerId == reservation.CustomerId);
    }

    [Fact]
    public async Task FindCustomersByPartySizeAsync_WithThresholdEqualToThePartySize_ExcludesTheCustomer()
    {
        var reservation = await AddReservationAsync(partySize: 8);

        var customers = await new CustomerRepository(Context).FindCustomersByPartySizeAsync(8);

        Assert.DoesNotContain(customers, customer => customer.CustomerId == reservation.CustomerId);
    }

    [Fact]
    public async Task FindCustomersByPartySizeAsync_WithSeveralMatchingReservations_ReturnsTheCustomerOnceWithEveryColumn()
    {
        var reservation = await AddReservationAsync(partySize: 9);

        Context.Reservations.Add(new Reservation
        {
            CustomerId = reservation.CustomerId,
            RestaurantId = reservation.RestaurantId,
            TableId = reservation.TableId,
            ReservationDate = new DateTime(2026, 10, 2, 19, 0, 0),
            PartySize = 10
        });
        await Context.SaveChangesAsync();
        Context.ChangeTracker.Clear();

        var found = Assert.Single(await new CustomerRepository(Context).FindCustomersByPartySizeAsync(8),
            customer => customer.CustomerId == reservation.CustomerId);

        Assert.Equal(reservation.Customer.FirstName, found.FirstName);
        Assert.Equal(reservation.Customer.LastName, found.LastName);
        Assert.Equal(reservation.Customer.Email, found.Email);
        Assert.Equal(reservation.Customer.PhoneNumber, found.PhoneNumber);
    }

    [Fact]
    public async Task FindCustomersByPartySizeAsync_WithCustomerThatHasNoReservations_ExcludesThatCustomer()
    {
        var customer = NewCustomer();
        Context.Customers.Add(customer);
        await Context.SaveChangesAsync();
        Context.ChangeTracker.Clear();

        var customers = await new CustomerRepository(Context).FindCustomersByPartySizeAsync(0);

        Assert.DoesNotContain(customers, found => found.CustomerId == customer.CustomerId);
    }

    [Fact]
    public async Task GetTotalRevenueAsync_WithOrdersAcrossSeveralRestaurants_SumsOnlyTheGivenRestaurant()
    {
        var reservation = await AddReservationAsync(orderTotal: 25.00m);
        var other = await AddReservationAsync(orderTotal: 999.00m);

        Context.Orders.Add(NewOrder(reservation.ReservationId, reservation.Orders.Single().EmployeeId, 75.50m));
        await Context.SaveChangesAsync();
        Context.ChangeTracker.Clear();

        var repository = new RestaurantRepository(Context);

        Assert.Equal(100.50m, await repository.GetTotalRevenueAsync(reservation.RestaurantId));
        Assert.Equal(999.00m, await repository.GetTotalRevenueAsync(other.RestaurantId));
    }

    [Fact]
    public async Task GetTotalRevenueAsync_WithRestaurantThatHasNoOrders_ReturnsZero()
    {
        var restaurant = NewRestaurant("Empty Bistro");
        Context.Restaurants.Add(restaurant);
        await Context.SaveChangesAsync();
        Context.ChangeTracker.Clear();

        Assert.Equal(0m, await new RestaurantRepository(Context).GetTotalRevenueAsync(restaurant.RestaurantId));
    }

    [Fact]
    public async Task GetTotalRevenueAsync_WithUnknownRestaurant_ReturnsZero()
    {
        Assert.Equal(0m, await new RestaurantRepository(Context).GetTotalRevenueAsync(-1));
    }

    [Fact]
    public async Task ListManagersAsync_WithMixedPositions_ReturnsOnlyTheManagers()
    {
        var manager = await AddReservationAsync(position: EmployeePosition.Manager);
        var waiter = await AddReservationAsync(position: EmployeePosition.AssistantWaiter);

        var managers = await new EmployeeRepository(Context).ListManagersAsync();

        Assert.Contains(managers, employee => employee.EmployeeId == manager.Orders.Single().EmployeeId);
        Assert.DoesNotContain(managers, employee => employee.EmployeeId == waiter.Orders.Single().EmployeeId);
        Assert.All(managers, employee => Assert.Equal(EmployeePosition.Manager, employee.Position));
    }

    [Fact]
    public async Task CalculateAverageOrderAmountAsync_WithOrdersFromSeveralEmployees_AveragesOnlyTheGivenEmployee()
    {
        var reservation = await AddReservationAsync(orderTotal: 25.00m);
        var employeeId = reservation.Orders.Single().EmployeeId;

        var colleague = new Employee
        {
            RestaurantId = reservation.RestaurantId,
            FirstName = "Other",
            LastName = "Waiter",
            Position = EmployeePosition.StandardWaiter
        };
        Context.Employees.Add(colleague);
        await Context.SaveChangesAsync();

        Context.Orders.Add(NewOrder(reservation.ReservationId, employeeId, 75.00m));
        Context.Orders.Add(NewOrder(reservation.ReservationId, colleague.EmployeeId, 500.00m));
        await Context.SaveChangesAsync();
        Context.ChangeTracker.Clear();

        var repository = new EmployeeRepository(Context);

        Assert.Equal(50.00m, await repository.CalculateAverageOrderAmountAsync(employeeId));
        Assert.Equal(500.00m, await repository.CalculateAverageOrderAmountAsync(colleague.EmployeeId));
    }

    [Fact]
    public async Task CalculateAverageOrderAmountAsync_WithEmployeeThatHasNoOrders_ReturnsZero()
    {
        var reservation = await AddReservationAsync();

        var newcomer = new Employee
        {
            RestaurantId = reservation.RestaurantId,
            FirstName = "New",
            LastName = "Employee",
            Position = EmployeePosition.StandardWaiter
        };
        Context.Employees.Add(newcomer);
        await Context.SaveChangesAsync();
        Context.ChangeTracker.Clear();

        Assert.Equal(0m, await new EmployeeRepository(Context).CalculateAverageOrderAmountAsync(newcomer.EmployeeId));
    }

    [Fact]
    public async Task CalculateAverageOrderAmountAsync_WithUnknownEmployee_ReturnsZero()
    {
        Assert.Equal(0m, await new EmployeeRepository(Context).CalculateAverageOrderAmountAsync(-1));
    }

    [Fact]
    public async Task GetEmployeesWithRestaurantAsync_WithEmployeesInTheDatabase_ReturnsOneViewRowPerEmployee()
    {
        var reservation = await AddReservationAsync();

        var details = await new EmployeeRepository(Context).GetEmployeesWithRestaurantAsync();
        var row = Assert.Single(details, detail => detail.EmployeeId == reservation.Orders.Single().EmployeeId);

        Assert.Equal(reservation.RestaurantId, row.RestaurantId);
        Assert.Equal("Test Bistro", row.RestaurantName);
        Assert.Equal("1 Test Street, Testville", row.RestaurantAddress);
        Assert.Equal("555-9000", row.RestaurantPhoneNumber);
        Assert.Equal("09:00-22:00", row.RestaurantOpeningHours);
        Assert.Equal(nameof(EmployeePosition.StandardWaiter), row.Position);
        Assert.Equal(await Context.Employees.CountAsync(), details.Count);
    }

    [Fact]
    public async Task GetReservationsByCustomerAsync_WithReservationsFromSeveralCustomers_ReturnsOnlyThatCustomersWithNavigations()
    {
        var reservation = await AddReservationAsync();

        var other = NewCustomer();
        Context.Customers.Add(other);
        await Context.SaveChangesAsync();

        Context.Reservations.Add(new Reservation
        {
            CustomerId = other.CustomerId,
            RestaurantId = reservation.RestaurantId,
            TableId = reservation.TableId,
            ReservationDate = new DateTime(2026, 10, 2, 19, 0, 0),
            PartySize = 4
        });
        await Context.SaveChangesAsync();
        Context.ChangeTracker.Clear();

        var found = Assert.Single(
            await new ReservationRepository(Context).GetReservationsByCustomerAsync(reservation.CustomerId));

        Assert.Equal(reservation.ReservationId, found.ReservationId);
        Assert.Equal("Test Bistro", found.Restaurant.Name);
        Assert.Equal(reservation.TableId, found.Table.TableId);
    }

    [Fact]
    public async Task GetReservationsByCustomerAsync_WithCustomerThatHasNoReservations_ReturnsEmpty()
    {
        var customer = NewCustomer();
        Context.Customers.Add(customer);
        await Context.SaveChangesAsync();
        Context.ChangeTracker.Clear();

        Assert.Empty(await new ReservationRepository(Context).GetReservationsByCustomerAsync(customer.CustomerId));
    }

    [Fact]
    public async Task GetReservationDetailsAsync_WithReservationsInTheDatabase_ReturnsOneViewRowPerReservation()
    {
        var reservation = await AddReservationAsync(partySize: 5);

        var details = await new ReservationRepository(Context).GetReservationDetailsAsync();
        var row = Assert.Single(details, detail => detail.ReservationId == reservation.ReservationId);

        Assert.Equal(5, row.PartySize);
        Assert.Equal(reservation.CustomerId, row.CustomerId);
        Assert.Equal(reservation.Customer.FirstName, row.CustomerFirstName);
        Assert.Equal(reservation.Customer.Email, row.CustomerEmail);
        Assert.Equal("Test Bistro", row.RestaurantName);
        Assert.Equal(await Context.Reservations.CountAsync(), details.Count);
    }

    [Fact]
    public async Task ListOrdersAndMenuItemsAsync_WithOrdersOnSeveralReservations_ReturnsOnlyThatReservationsOrdersWithTheirMenuItems()
    {
        var reservation = await AddReservationAsync(menuItemPrice: 12.34m);
        await AddReservationAsync();

        var orders = await new OrderRepository(Context).ListOrdersAndMenuItemsAsync(reservation.ReservationId);

        var order = Assert.Single(orders);
        Assert.Equal(reservation.Orders.Single().OrderId, order.OrderId);

        var orderItem = Assert.Single(order.OrderItems);
        Assert.Equal("Test Dish", orderItem.MenuItem.Name);
        Assert.Equal(12.34m, orderItem.MenuItem.Price);
    }

    [Fact]
    public async Task ListOrdersAndMenuItemsAsync_WithUnknownReservation_ReturnsEmpty()
    {
        Assert.Empty(await new OrderRepository(Context).ListOrdersAndMenuItemsAsync(-1));
    }

    [Fact]
    public async Task ListOrderedMenuItemsAsync_WithRepeatedAndDistinctItems_ReturnsEachItemOnce()
    {
        var reservation = await AddReservationAsync();
        var order = reservation.Orders.Single();
        var dish = order.OrderItems.Single().MenuItem;

        var dessert = new MenuItem
        {
            RestaurantId = reservation.RestaurantId,
            Name = "Test Dessert",
            Price = 6.50m
        };
        Context.MenuItems.Add(dessert);

        var secondOrder = NewOrder(reservation.ReservationId, order.EmployeeId, 30.00m);
        Context.Orders.Add(secondOrder);
        await Context.SaveChangesAsync();

        Context.OrderItems.Add(new OrderItem { OrderId = secondOrder.OrderId, ItemId = dish.ItemId, Quantity = 3 });
        Context.OrderItems.Add(new OrderItem { OrderId = order.OrderId, ItemId = dessert.ItemId, Quantity = 1 });
        await Context.SaveChangesAsync();
        Context.ChangeTracker.Clear();

        var menuItems = await new OrderRepository(Context).ListOrderedMenuItemsAsync(reservation.ReservationId);

        Assert.Equal(2, menuItems.Count);
        Assert.Contains(menuItems, item => item.ItemId == dish.ItemId);
        Assert.Contains(menuItems, item => item.ItemId == dessert.ItemId);
    }

    [Fact]
    public async Task ListOrderedMenuItemsAsync_WithUnknownReservation_ReturnsEmpty()
    {
        Assert.Empty(await new OrderRepository(Context).ListOrderedMenuItemsAsync(-1));
    }
}
