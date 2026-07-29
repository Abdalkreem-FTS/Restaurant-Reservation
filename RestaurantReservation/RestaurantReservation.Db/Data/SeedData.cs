using RestaurantReservation.Db.Entities;
using RestaurantReservation.Db.Enums;

namespace RestaurantReservation.Db.Data;

public static class SeedData
{
    private static readonly Restaurant[] _restaurants =
    [
        new()
        {
            RestaurantId = 1, Name = "The Gourmet Kitchen", Address = "123 Main St, Springfield",
            PhoneNumber = "555-0101", OpeningHours = "10:00-23:00"
        },
        new()
        {
            RestaurantId = 2, Name = "Bella Italia", Address = "45 Oak Avenue, Springfield", PhoneNumber = "555-0102",
            OpeningHours = "11:00-22:00"
        },
        new()
        {
            RestaurantId = 3, Name = "Sushi Zen", Address = "9 River Road, Shelbyville", PhoneNumber = "555-0103",
            OpeningHours = "12:00-22:30"
        },
        new()
        {
            RestaurantId = 4, Name = "Le Petite Bistro", Address = "78 Elm Street, Shelbyville",
            PhoneNumber = "555-0104", OpeningHours = "17:00-23:30"
        },
        new()
        {
            RestaurantId = 5, Name = "The Steakhouse", Address = "200 Grand Blvd, Capital City",
            PhoneNumber = "555-0105", OpeningHours = "16:00-00:00"
        }
    ];

    private static readonly Customer[] _customers =
    [
        new()
        {
            CustomerId = 1, FirstName = "John", LastName = "Doe", Email = "john.doe@example.com",
            PhoneNumber = "555-1001"
        },
        new()
        {
            CustomerId = 2, FirstName = "Jane", LastName = "Smith", Email = "jane.smith@example.com",
            PhoneNumber = "555-1002"
        },
        new()
        {
            CustomerId = 3, FirstName = "Michael", LastName = "Johnson", Email = "michael.johnson@example.com",
            PhoneNumber = "555-1003"
        },
        new()
        {
            CustomerId = 4, FirstName = "Emily", LastName = "Davis", Email = "emily.davis@example.com",
            PhoneNumber = "555-1004"
        },
        new()
        {
            CustomerId = 5, FirstName = "David", LastName = "Wilson", Email = "david.wilson@example.com",
            PhoneNumber = "555-1005"
        }
    ];

    private static readonly Table[] _tables =
    [
        new() { TableId = 1, RestaurantId = 1, Capacity = 2 },
        new() { TableId = 2, RestaurantId = 1, Capacity = 4 },
        new() { TableId = 3, RestaurantId = 2, Capacity = 4 },
        new() { TableId = 4, RestaurantId = 3, Capacity = 6 },
        new() { TableId = 5, RestaurantId = 4, Capacity = 2 },
        new() { TableId = 6, RestaurantId = 5, Capacity = 8 }
    ];

    private static readonly Employee[] _employees =
    [
        new()
        {
            EmployeeId = 1, RestaurantId = 1, FirstName = "Alice", LastName = "Turner",
            Position = EmployeePosition.Manager
        },
        new()
        {
            EmployeeId = 2, RestaurantId = 1, FirstName = "Bob", LastName = "Cook", Position = EmployeePosition.VipOrdersWaiter
        },
        new()
        {
            EmployeeId = 3, RestaurantId = 2, FirstName = "Carol", LastName = "White",
            Position = EmployeePosition.Manager
        },
        new()
        {
            EmployeeId = 4, RestaurantId = 2, FirstName = "Dan", LastName = "Brown", Position = EmployeePosition.StandardWaiter
        },
        new()
        {
            EmployeeId = 5, RestaurantId = 3, FirstName = "Eve", LastName = "Black", Position = EmployeePosition.Manager
        },
        new()
        {
            EmployeeId = 6, RestaurantId = 4, FirstName = "Frank", LastName = "Green",
            Position = EmployeePosition.AssistantWaiter
        }
    ];

    private static readonly MenuItem[] _menuItems =
    [
        new()
        {
            ItemId = 1, RestaurantId = 1, Name = "Grilled Salmon", Description = "Atlantic salmon with lemon butter",
            Price = 24.99m
        },
        new()
        {
            ItemId = 2, RestaurantId = 1, Name = "Caesar Salad", Description = "Romaine, parmesan, croutons",
            Price = 12.50m
        },
        new()
        {
            ItemId = 3, RestaurantId = 2, Name = "Margherita Pizza", Description = "Tomato, mozzarella, basil",
            Price = 15.00m
        },
        new()
        {
            ItemId = 4, RestaurantId = 2, Name = "Spaghetti Carbonara", Description = "Egg, pancetta, pecorino",
            Price = 17.50m
        },
        new()
        {
            ItemId = 5, RestaurantId = 3, Name = "Sushi Platter", Description = "Chef's selection of 12 pieces",
            Price = 29.99m
        },
        new()
        {
            ItemId = 6, RestaurantId = 4, Name = "Beef Bourguignon", Description = "Slow-braised beef in red wine",
            Price = 26.00m
        }
    ];

    private static readonly Reservation[] _reservations =
    [
        new()
        {
            ReservationId = 1, CustomerId = 1, RestaurantId = 1, TableId = 1,
            ReservationDate = new DateTime(2026, 8, 1, 19, 0, 0), PartySize = 2, TableCapacity = 2
        },
        new()
        {
            ReservationId = 2, CustomerId = 2, RestaurantId = 1, TableId = 2,
            ReservationDate = new DateTime(2026, 8, 2, 20, 0, 0), PartySize = 4, TableCapacity = 4
        },
        new()
        {
            ReservationId = 3, CustomerId = 3, RestaurantId = 2, TableId = 3,
            ReservationDate = new DateTime(2026, 8, 3, 18, 0, 0), PartySize = 3, TableCapacity = 4
        },
        new()
        {
            ReservationId = 4, CustomerId = 4, RestaurantId = 3, TableId = 4,
            ReservationDate = new DateTime(2026, 8, 4, 19, 0, 0), PartySize = 6, TableCapacity = 6
        },
        new()
        {
            ReservationId = 5, CustomerId = 5, RestaurantId = 4, TableId = 5,
            ReservationDate = new DateTime(2026, 8, 5, 20, 0, 0), PartySize = 2, TableCapacity = 2
        },
        new()
        {
            ReservationId = 6, CustomerId = 1, RestaurantId = 5, TableId = 6,
            ReservationDate = new DateTime(2026, 8, 6, 21, 0, 0), PartySize = 8, TableCapacity = 8
        }
    ];

    private static readonly Order[] _orders =
    [
        new()
        {
            OrderId = 1, ReservationId = 1, EmployeeId = 1, RestaurantId = 1,
            OrderDate = new DateTime(2026, 8, 1, 19, 30, 0), TotalAmount = 37.49m
        },
        new()
        {
            OrderId = 2, ReservationId = 1, EmployeeId = 2, RestaurantId = 1,
            OrderDate = new DateTime(2026, 8, 1, 20, 0, 0), TotalAmount = 24.99m
        },
        new()
        {
            OrderId = 3, ReservationId = 2, EmployeeId = 1, RestaurantId = 1,
            OrderDate = new DateTime(2026, 8, 2, 20, 30, 0), TotalAmount = 49.98m
        },
        new()
        {
            OrderId = 4, ReservationId = 3, EmployeeId = 3, RestaurantId = 2,
            OrderDate = new DateTime(2026, 8, 3, 19, 0, 0), TotalAmount = 32.50m
        },
        new()
        {
            OrderId = 5, ReservationId = 4, EmployeeId = 5, RestaurantId = 3,
            OrderDate = new DateTime(2026, 8, 4, 20, 0, 0), TotalAmount = 29.99m
        },
        new()
        {
            OrderId = 6, ReservationId = 5, EmployeeId = 6, RestaurantId = 4,
            OrderDate = new DateTime(2026, 8, 5, 20, 30, 0), TotalAmount = 26.00m
        }
    ];

    private static readonly OrderItem[] _orderItems =
    [
        new() { OrderItemId = 1, OrderId = 1, ItemId = 1, RestaurantId = 1, Quantity = 1, UnitPrice = 24.99m },
        new() { OrderItemId = 2, OrderId = 1, ItemId = 2, RestaurantId = 1, Quantity = 1, UnitPrice = 12.50m },
        new() { OrderItemId = 3, OrderId = 2, ItemId = 1, RestaurantId = 1, Quantity = 1, UnitPrice = 24.99m },
        new() { OrderItemId = 4, OrderId = 3, ItemId = 1, RestaurantId = 1, Quantity = 2, UnitPrice = 24.99m },
        new() { OrderItemId = 5, OrderId = 4, ItemId = 3, RestaurantId = 2, Quantity = 1, UnitPrice = 15.00m },
        new() { OrderItemId = 6, OrderId = 4, ItemId = 4, RestaurantId = 2, Quantity = 1, UnitPrice = 17.50m },
        new() { OrderItemId = 7, OrderId = 5, ItemId = 5, RestaurantId = 3, Quantity = 1, UnitPrice = 29.99m },
        new() { OrderItemId = 8, OrderId = 6, ItemId = 6, RestaurantId = 4, Quantity = 1, UnitPrice = 26.00m }
    ];

    public static ModelBuilder Seed(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Restaurant>().HasData(_restaurants);
        modelBuilder.Entity<Customer>().HasData(_customers);
        modelBuilder.Entity<Table>().HasData(_tables);
        modelBuilder.Entity<Employee>().HasData(_employees);
        modelBuilder.Entity<MenuItem>().HasData(_menuItems);
        modelBuilder.Entity<Reservation>().HasData(_reservations);
        modelBuilder.Entity<Order>().HasData(_orders);
        modelBuilder.Entity<OrderItem>().HasData(_orderItems);

        return modelBuilder;
    }
}
