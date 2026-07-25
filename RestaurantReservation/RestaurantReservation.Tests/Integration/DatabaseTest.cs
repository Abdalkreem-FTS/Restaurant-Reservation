using Microsoft.EntityFrameworkCore.Storage;
using RestaurantReservation.Db.Enums;

namespace RestaurantReservation.Tests.Integration;

[Trait("Category", "Integration")]
public abstract class DatabaseTest(SqlServerFixture fixture) : IAsyncLifetime
{
    private IDbContextTransaction _transaction = null!;

    protected RestaurantReservationDbContext Context { get; } = fixture.CreateContext();

    public async Task InitializeAsync() => _transaction = await Context.Database.BeginTransactionAsync();

    public async Task DisposeAsync()
    {
        await _transaction.RollbackAsync();
        await Context.DisposeAsync();
    }
    
    protected async Task<Reservation> AddReservationAsync(int partySize = 2, decimal orderTotal = 25.00m, decimal menuItemPrice = 10.00m, EmployeePosition position = EmployeePosition.StandardWaiter)
    {
        var restaurant = NewRestaurant();

        var reservation = new Reservation
        {
            Customer = NewCustomer(),
            Restaurant = restaurant,
            Table = new Table { Restaurant = restaurant, Capacity = 4 },
            ReservationDate = new DateTime(2026, 10, 1, 19, 0, 0),
            PartySize = partySize,
            Orders =
            {
                new Order
                {
                    Employee = new Employee
                    {
                        Restaurant = restaurant,
                        FirstName = "Test",
                        LastName = "Employee",
                        Position = position
                    },
                    OrderDate = new DateTime(2026, 10, 1, 19, 30, 0),
                    TotalAmount = orderTotal,
                    OrderItems =
                    {
                        new OrderItem
                        {
                            MenuItem = new MenuItem
                            {
                                Restaurant = restaurant,
                                Name = "Test Dish",
                                Description = "Created by an integration test",
                                Price = menuItemPrice
                            },
                            Quantity = 1
                        }
                    }
                }
            }
        };

        Context.Reservations.Add(reservation);
        await Context.SaveChangesAsync();
        Context.ChangeTracker.Clear();

        return reservation;
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

    protected static Order NewOrder(int reservationId, int employeeId, decimal totalAmount) => new()
    {
        ReservationId = reservationId,
        EmployeeId = employeeId,
        OrderDate = new DateTime(2026, 10, 1, 20, 0, 0),
        TotalAmount = totalAmount
    };
}
