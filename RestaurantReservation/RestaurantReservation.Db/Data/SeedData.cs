using RestaurantReservation.Db.Entities;
using RestaurantReservation.Db.Enums;

namespace RestaurantReservation.Db.Data;

public static class SeedData
{
    private static readonly Restaurant[] Restaurants =
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

    private static readonly User[] Users =
    [
        new() { UserId = 1, Username = "john.doe", PasswordHash = "AQAAAAIAAYagAAAAEIBPg0ewhJ5n53xz44KlYSSDvhwxyBuRtzQ+ytXmpEao5y/dmWmFOp9jKOJa7ux+bQ==" },
        new() { UserId = 2, Username = "jane.smith", PasswordHash = "AQAAAAIAAYagAAAAEDm7Y7HkVS/kaIjfrvDj74Oca+8J1GYa7mpUjuUg2ag4HQvo4Jk6tIm47/VGnbmkyw==" },
        new() { UserId = 3, Username = "michael.johnson", PasswordHash = "AQAAAAIAAYagAAAAEKBdwZsSAeO6CJrYUhTx74qhMv3YxDO/loMRtHxIs7XFN51qIocdI9jRe0Q98PYOTQ==" },
        new() { UserId = 4, Username = "emily.davis", PasswordHash = "AQAAAAIAAYagAAAAEOTo3Ly50M1eu49u3Mf9qr+NkRO2QRWcNRjZDqBUfNRLz0gEgCsSS4dm4a7B6TCJJg==" },
        new() { UserId = 5, Username = "david.wilson", PasswordHash = "AQAAAAIAAYagAAAAEBKJGhoqk3DWchXBg6SQULteviXT1GatQtEXHqWnto6/LBam3IdGQWDibFUgf/yk1w==" },
        new() { UserId = 6, Username = "alice.turner", PasswordHash = "AQAAAAIAAYagAAAAENo/A93E/1FhZg8+OAnOLm70xAqSjW6clyn4AJIpCH4GcsBwuJaxf5UEA2oZ4UQjSQ==" },
        new() { UserId = 7, Username = "bob.cook", PasswordHash = "AQAAAAIAAYagAAAAEExphGBVuZExPZTlHQ6hQO1GT+nyON6koGot2siIyRA1lE1WzPDn0h1xXAXbpR3Yjw==" },
        new() { UserId = 8, Username = "carol.white", PasswordHash = "AQAAAAIAAYagAAAAELVW31dCx40O1GBijQ7Upeu9sGZVfCHyQhBLqGR7P9vXO0hcsmrRNx8uuhNB2zQ61Q==" },
        new() { UserId = 9, Username = "dan.brown", PasswordHash = "AQAAAAIAAYagAAAAEDrJQV8EwE1UtxJtz6GE2j0AHcNTlzBiFcKt8cwXwWC3oBHA95ImfdMgaeXnZs8IRQ==" },
        new() { UserId = 10, Username = "eve.black", PasswordHash = "AQAAAAIAAYagAAAAEMgwF8w+eAczEKjurnAoIhjX+BxqRpaZxALyFd3XRU3UIcMI/MVHMQRmLPwmkDSX5Q==" },
        new() { UserId = 11, Username = "frank.green", PasswordHash = "AQAAAAIAAYagAAAAEP0ocSpAttnNpNcwKkhuNIFho6xB3NNWy/FFZp/XaKuS5p3uUkeQ3DfjfmQ/klby4g==" },
        new() { UserId = 12, Username = "grace.hall", PasswordHash = "AQAAAAIAAYagAAAAEAXattSIS4ApP9nf7qxL7QLK2TVDauevYDfGbUsZAYtUDRxJ0/LweHYhuNj/ReWHEg==" }
    ];

    private static readonly Customer[] Customers =
    [
        new()
        {
            CustomerId = 1, UserId = 1, FirstName = "John", LastName = "Doe", Email = "john.doe@example.com",
            PhoneNumber = "555-1001"
        },
        new()
        {
            CustomerId = 2, UserId = 2, FirstName = "Jane", LastName = "Smith", Email = "jane.smith@example.com",
            PhoneNumber = "555-1002"
        },
        new()
        {
            CustomerId = 3, UserId = 3, FirstName = "Michael", LastName = "Johnson", Email = "michael.johnson@example.com",
            PhoneNumber = "555-1003"
        },
        new()
        {
            CustomerId = 4, UserId = 4, FirstName = "Emily", LastName = "Davis", Email = "emily.davis@example.com",
            PhoneNumber = "555-1004"
        },
        new()
        {
            CustomerId = 5, UserId = 5, FirstName = "David", LastName = "Wilson", Email = "david.wilson@example.com",
            PhoneNumber = "555-1005"
        }
    ];

    private static readonly Table[] Tables =
    [
        new() { TableId = 1, RestaurantId = 1, Capacity = 2 },
        new() { TableId = 2, RestaurantId = 1, Capacity = 4 },
        new() { TableId = 3, RestaurantId = 2, Capacity = 4 },
        new() { TableId = 4, RestaurantId = 3, Capacity = 6 },
        new() { TableId = 5, RestaurantId = 4, Capacity = 2 },
        new() { TableId = 6, RestaurantId = 5, Capacity = 8 }
    ];

    private static readonly Employee[] Employees =
    [
        new()
        {
            EmployeeId = 1, UserId = 6, RestaurantId = 1, FirstName = "Alice", LastName = "Turner",
            Position = EmployeePosition.Manager
        },
        new()
        {
            EmployeeId = 2, UserId = 7, RestaurantId = 1, FirstName = "Bob", LastName = "Cook", Position = EmployeePosition.VipOrdersWaiter
        },
        new()
        {
            EmployeeId = 3, UserId = 8, RestaurantId = 2, FirstName = "Carol", LastName = "White",
            Position = EmployeePosition.Manager
        },
        new()
        {
            EmployeeId = 4, UserId = 9, RestaurantId = 2, FirstName = "Dan", LastName = "Brown", Position = EmployeePosition.StandardWaiter
        },
        new()
        {
            EmployeeId = 5, UserId = 10, RestaurantId = 3, FirstName = "Eve", LastName = "Black", Position = EmployeePosition.Manager
        },
        new()
        {
            EmployeeId = 6, UserId = 11, RestaurantId = 4, FirstName = "Frank", LastName = "Green",
            Position = EmployeePosition.AssistantWaiter
        },
        new()
        {
            EmployeeId = 7, UserId = 12, RestaurantId = 1, FirstName = "Grace", LastName = "Hall",
            Position = EmployeePosition.Admin
        }
    ];

    private static readonly MenuItem[] MenuItems =
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

    private static readonly Reservation[] Reservations =
    [
        new()
        {
            ReservationId = 1, CustomerId = 1, RestaurantId = 1, TableId = 1,
            ReservationDate = new DateTimeOffset(2026, 8, 1, 19, 0, 0, TimeSpan.Zero), PartySize = 2, TableCapacity = 2
        },
        new()
        {
            ReservationId = 2, CustomerId = 2, RestaurantId = 1, TableId = 2,
            ReservationDate = new DateTimeOffset(2026, 8, 2, 20, 0, 0, TimeSpan.Zero), PartySize = 4, TableCapacity = 4
        },
        new()
        {
            ReservationId = 3, CustomerId = 3, RestaurantId = 2, TableId = 3,
            ReservationDate = new DateTimeOffset(2026, 8, 3, 18, 0, 0, TimeSpan.Zero), PartySize = 3, TableCapacity = 4
        },
        new()
        {
            ReservationId = 4, CustomerId = 4, RestaurantId = 3, TableId = 4,
            ReservationDate = new DateTimeOffset(2026, 8, 4, 19, 0, 0, TimeSpan.Zero), PartySize = 6, TableCapacity = 6
        },
        new()
        {
            ReservationId = 5, CustomerId = 5, RestaurantId = 4, TableId = 5,
            ReservationDate = new DateTimeOffset(2026, 8, 5, 20, 0, 0, TimeSpan.Zero), PartySize = 2, TableCapacity = 2
        },
        new()
        {
            ReservationId = 6, CustomerId = 1, RestaurantId = 5, TableId = 6,
            ReservationDate = new DateTimeOffset(2026, 8, 6, 21, 0, 0, TimeSpan.Zero), PartySize = 8, TableCapacity = 8
        }
    ];

    private static readonly Order[] Orders =
    [
        new()
        {
            OrderId = 1, ReservationId = 1, EmployeeId = 1, RestaurantId = 1,
            OrderDate = new DateTimeOffset(2026, 8, 1, 19, 30, 0, TimeSpan.Zero), TotalAmount = 37.49m
        },
        new()
        {
            OrderId = 2, ReservationId = 1, EmployeeId = 2, RestaurantId = 1,
            OrderDate = new DateTimeOffset(2026, 8, 1, 20, 0, 0, TimeSpan.Zero), TotalAmount = 24.99m
        },
        new()
        {
            OrderId = 3, ReservationId = 2, EmployeeId = 1, RestaurantId = 1,
            OrderDate = new DateTimeOffset(2026, 8, 2, 20, 30, 0, TimeSpan.Zero), TotalAmount = 49.98m
        },
        new()
        {
            OrderId = 4, ReservationId = 3, EmployeeId = 3, RestaurantId = 2,
            OrderDate = new DateTimeOffset(2026, 8, 3, 19, 0, 0, TimeSpan.Zero), TotalAmount = 32.50m
        },
        new()
        {
            OrderId = 5, ReservationId = 4, EmployeeId = 5, RestaurantId = 3,
            OrderDate = new DateTimeOffset(2026, 8, 4, 20, 0, 0, TimeSpan.Zero), TotalAmount = 29.99m
        },
        new()
        {
            OrderId = 6, ReservationId = 5, EmployeeId = 6, RestaurantId = 4,
            OrderDate = new DateTimeOffset(2026, 8, 5, 20, 30, 0, TimeSpan.Zero), TotalAmount = 26.00m
        }
    ];

    private static readonly OrderItem[] OrderItems =
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
        modelBuilder.Entity<User>().HasData(Users);
        modelBuilder.Entity<Restaurant>().HasData(Restaurants);
        modelBuilder.Entity<Customer>().HasData(Customers);
        modelBuilder.Entity<Table>().HasData(Tables);
        modelBuilder.Entity<Employee>().HasData(Employees);
        modelBuilder.Entity<MenuItem>().HasData(MenuItems);
        modelBuilder.Entity<Reservation>().HasData(Reservations);
        modelBuilder.Entity<Order>().HasData(Orders);
        modelBuilder.Entity<OrderItem>().HasData(OrderItems);

        return modelBuilder;
    }
}
