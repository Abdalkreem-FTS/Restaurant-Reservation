using RestaurantReservation.Db.Enums;

namespace RestaurantReservation.Demos;

public sealed class CrudDemo(Func<RestaurantReservationDbContext> newContext)
{
    public async Task RunAsync(SampleIds ids, CancellationToken cancellationToken)
    {
        await AddUpdateDeleteAsync(
            context => new RestaurantRepository(context),
            new Restaurant
            {
                Name = "Demo Diner",
                Address = "1 Demo Way, Springfield",
                PhoneNumber = "555-0199",
                OpeningHours = "09:00-21:00"
            },
            restaurant => restaurant.OpeningHours = "08:00-23:00",
            restaurant => $"Restaurant #{restaurant.RestaurantId} {restaurant.Name}, open {restaurant.OpeningHours}",
            cancellationToken);

        await AddUpdateDeleteAsync(
            context => new CustomerRepository(context),
            new Customer
            {
                FirstName = "Sam",
                LastName = "Reed",
                Email = "sam.reed@example.com",
                PhoneNumber = "555-1099"
            },
            customer => customer.Email = "sam.reed@updated.example.com",
            customer => $"Customer #{customer.CustomerId} {customer.FirstName} {customer.LastName} <{customer.Email}>",
            cancellationToken);

        await AddUpdateDeleteAsync(
            context => new TableRepository(context),
            new Table { RestaurantId = ids.RestaurantId, Capacity = 4 },
            table => table.Capacity = 6,
            table => $"Table #{table.TableId} at restaurant {table.RestaurantId}, seats {table.Capacity}",
            cancellationToken);

        await AddUpdateDeleteAsync(
            context => new EmployeeRepository(context),
            new Employee
            {
                RestaurantId = ids.RestaurantId,
                FirstName = "Demo",
                LastName = "Waiter",
                Position = EmployeePosition.StandardWaiter
            },
            employee => employee.Position = EmployeePosition.Manager,
            employee => $"Employee #{employee.EmployeeId} {employee.FirstName} {employee.LastName}, {employee.Position}",
            cancellationToken);

        await AddUpdateDeleteAsync(
            context => new MenuItemRepository(context),
            new MenuItem
            {
                RestaurantId = ids.RestaurantId,
                Name = "Demo Special",
                Description = "Chef's demonstration plate",
                Price = 19.99m
            },
            menuItem => menuItem.Price = 21.50m,
            menuItem => $"MenuItem #{menuItem.ItemId} {menuItem.Name}, {menuItem.Price:N2}",
            cancellationToken);

        await AddUpdateDeleteAsync(
            context => new ReservationRepository(context),
            new Reservation
            {
                CustomerId = ids.CustomerId,
                RestaurantId = ids.RestaurantId,
                TableId = ids.TableId,
                ReservationDate = new DateTime(2026, 9, 1, 19, 0, 0),
                PartySize = 2
            },
            reservation => reservation.PartySize = 4,
            reservation => $"Reservation #{reservation.ReservationId} on {reservation.ReservationDate:yyyy-MM-dd HH:mm} for {reservation.PartySize}",
            cancellationToken);

        await AddUpdateDeleteAsync(
            context => new OrderRepository(context),
            new Order
            {
                ReservationId = ids.ReservationId,
                EmployeeId = ids.EmployeeId,
                OrderDate = new DateTime(2026, 9, 1, 19, 30, 0),
                TotalAmount = 42.00m
            },
            order => order.TotalAmount = 55.25m,
            order => $"Order #{order.OrderId} on reservation {order.ReservationId}, total {order.TotalAmount:N2}",
            cancellationToken);

        await AddUpdateDeleteAsync(
            context => new OrderItemRepository(context),
            new OrderItem { OrderId = ids.OrderId, ItemId = ids.MenuItemId, Quantity = 2 },
            orderItem => orderItem.Quantity = 3,
            orderItem => $"OrderItem #{orderItem.OrderItemId} on order {orderItem.OrderId}, item {orderItem.ItemId} x{orderItem.Quantity}",
            cancellationToken);
    }
    
    private async Task AddUpdateDeleteAsync<TEntity>(
        Func<RestaurantReservationDbContext, IRepository<TEntity>> repository,
        TEntity entity,
        Action<TEntity> update,
        Func<TEntity, string> describe,
        CancellationToken cancellationToken) where TEntity : class
    {
        var label = typeof(TEntity).Name;

        ConsoleWriter.Step($"{label}: Add -> Update -> Delete");

        int id;

        await using (var context = newContext())
        {
            var created = await repository(context).AddAsync(entity, cancellationToken);

            id = PrimaryKeyOf(context, created);
            ConsoleWriter.Created(describe(created));
        }

        await using (var context = newContext())
        {
            var repositoryForUpdate = repository(context);
            var stored = await repositoryForUpdate.GetByIdAsync(id, cancellationToken) ?? throw new InvalidOperationException($"{label} #{id} was not persisted.");

            update(stored);

            await repositoryForUpdate.UpdateAsync(stored, cancellationToken);
        }

        await using (var context = newContext())
        {
            var updated = await repository(context).GetByIdAsync(id, cancellationToken) ?? throw new InvalidOperationException($"{label} #{id} disappeared after the update.");

            ConsoleWriter.Updated(describe(updated));
        }

        await using (var context = newContext())
        {
            await repository(context).DeleteAsync(id, cancellationToken);
        }

        await using (var context = newContext())
        {
            if (await repository(context).GetByIdAsync(id, cancellationToken) is not null)
            {
                throw new InvalidOperationException($"{label} #{id} still exists after the delete.");
            }

            ConsoleWriter.Deleted($"{label} #{id}");
        }
    }

    private static int PrimaryKeyOf<TEntity>(RestaurantReservationDbContext context, TEntity entity) where TEntity : class
    {
        var entry = context.Entry(entity);

        return (int)entry.Property(entry.Metadata.FindPrimaryKey()!.Properties[0].Name).CurrentValue!;
    }
}
