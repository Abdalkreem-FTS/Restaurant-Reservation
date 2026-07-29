using Microsoft.Extensions.Logging.Abstractions;

namespace RestaurantReservation.Tests.Integration;

[Trait("Category", "Integration")]
public abstract class DatabaseTest : IAsyncLifetime
{
    protected static readonly DateTime DefaultSlot = new(2026, 10, 1, 19, 0, 0);

    private readonly DatabaseFixture _fixture;
    private readonly IUnitOfWork _unitOfWork;

    protected DatabaseTest(DatabaseFixture fixture)
    {
        _fixture = fixture;

        Context = fixture.CreateContext();
        _unitOfWork = new UnitOfWork(Context, NullLogger<UnitOfWork>.Instance);

        Customers = new CustomerRepository(Context, NullLogger<CustomerRepository>.Instance);
        Restaurants = new RestaurantRepository(Context, NullLogger<RestaurantRepository>.Instance);
        Tables = new TableRepository(Context, NullLogger<TableRepository>.Instance);
        Employees = new EmployeeRepository(Context, NullLogger<EmployeeRepository>.Instance);
        MenuItems = new MenuItemRepository(Context, NullLogger<MenuItemRepository>.Instance);
        Reservations = new ReservationRepository(Context, NullLogger<ReservationRepository>.Instance);
        Orders = new OrderRepository(Context, NullLogger<OrderRepository>.Instance);
        OrderItems = new OrderItemRepository(Context, NullLogger<OrderItemRepository>.Instance);
    }

    protected RestaurantReservationDbContext Context { get; }

    protected ICustomerRepository Customers { get; }

    protected IRestaurantRepository Restaurants { get; }

    protected ITableRepository Tables { get; }

    protected IEmployeeRepository Employees { get; }

    protected IMenuItemRepository MenuItems { get; }

    protected IReservationRepository Reservations { get; }

    protected IOrderRepository Orders { get; }

    protected IOrderItemRepository OrderItems { get; }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync() => await Context.DisposeAsync();

    protected RestaurantReservationDbContext NewContext() => _fixture.CreateContext();

    protected async Task<int> SaveAsync()
    {
        var result = await _unitOfWork.SaveChangesAsync();

        Assert.True(result.IsSuccess, result.IsError ? result.TopError.Description : null);

        return result.Value;
    }

    protected Task<Result<int>> TrySaveAsync() => _unitOfWork.SaveChangesAsync();

    protected async Task<RestaurantSetup> AddRestaurantSetupAsync(
        int tableCapacity = 4,
        decimal menuItemPrice = 10.00m,
        EmployeePosition position = EmployeePosition.StandardWaiter)
    {
        var restaurant = NewRestaurant();
        var table = new Table { Restaurant = restaurant, Capacity = tableCapacity };
        var employee = new Employee
        {
            Restaurant = restaurant,
            FirstName = "Test",
            LastName = "Employee",
            Position = position
        };
        var menuItem = new MenuItem
        {
            Restaurant = restaurant,
            Name = "Test Dish",
            Description = "Created by an integration test",
            Price = menuItemPrice
        };

        Context.AddRange(restaurant, table, employee, menuItem);

        await SaveAsync();

        Context.ChangeTracker.Clear();

        return new RestaurantSetup(restaurant, table, employee, menuItem);
    }

    protected async Task<Reservation> AddReservationAsync(
        RestaurantSetup setup,
        int partySize = 2,
        DateTime? at = null,
        int? customerId = null)
    {
        var reservation = NewReservation(setup, partySize, at);

        if (customerId is null)
        {
            reservation.Customer = NewCustomer();
        }
        else
        {
            reservation.CustomerId = customerId.Value;
        }

        Context.Add(reservation);

        await SaveAsync();

        Context.ChangeTracker.Clear();

        return reservation;
    }

    protected async Task<Order> AddOrderAsync(
        RestaurantSetup setup,
        Reservation reservation,
        int quantity = 1,
        decimal? unitPrice = null)
    {
        var order = NewOrder(setup, reservation, quantity, unitPrice);

        Context.Add(order);

        await SaveAsync();

        Context.ChangeTracker.Clear();

        return order;
    }

    protected static Reservation NewReservation(RestaurantSetup setup, int partySize = 2, DateTime? at = null) => new()
    {
        RestaurantId = setup.RestaurantId,
        TableId = setup.Table.TableId,
        TableCapacity = setup.Table.Capacity,
        ReservationDate = at ?? DefaultSlot,
        PartySize = partySize
    };

    protected static Order NewOrder(RestaurantSetup setup, Reservation reservation, int quantity = 1, decimal? unitPrice = null)
    {
        var order = new Order
        {
            ReservationId = reservation.ReservationId,
            EmployeeId = setup.Employee.EmployeeId,
            RestaurantId = setup.RestaurantId,
            OrderDate = reservation.ReservationDate.AddMinutes(30),
            OrderItems =
            {
                new OrderItem
                {
                    ItemId = setup.MenuItem.ItemId,
                    RestaurantId = setup.RestaurantId,
                    Quantity = quantity,
                    UnitPrice = unitPrice ?? setup.MenuItem.Price
                }
            }
        };

        order.RecalculateTotal();

        return order;
    }

    protected static Restaurant NewRestaurant(string name = "Test Bistro") => new()
    {
        Name = name,
        Address = "1 Test Street, Testville",
        PhoneNumber = "555-9000",
        OpeningHours = "09:00-22:00"
    };

    protected static Customer NewCustomer() => new()
    {
        FirstName = "Test",
        LastName = "Customer",
        Email = $"test.{Guid.NewGuid():N}@example.com",
        PhoneNumber = "555-9001"
    };

    protected sealed record RestaurantSetup(Restaurant Restaurant, Table Table, Employee Employee, MenuItem MenuItem)
    {
        public int RestaurantId => Restaurant.RestaurantId;
    }
}
