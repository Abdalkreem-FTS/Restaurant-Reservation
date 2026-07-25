namespace RestaurantReservation.Demos;

public sealed class QueryDemo(Func<RestaurantReservationDbContext> newContext)
{
    public async Task RunAsync(SampleIds ids, CancellationToken cancellationToken)
    {
        await using var context = newContext();

        var employees = new EmployeeRepository(context);
        var reservations = new ReservationRepository(context);
        var orders = new OrderRepository(context);

        ConsoleWriter.Step("ListManagers()");

        var managers = await employees.ListManagersAsync(cancellationToken);

        ConsoleWriter.Table(
            ["EmployeeId", "Name", "Position", "RestaurantId"],
            managers.Select(manager => new[]
            {
                manager.EmployeeId.ToString(),
                $"{manager.FirstName} {manager.LastName}",
                manager.Position.ToString(),
                manager.RestaurantId.ToString()
            }));

        ConsoleWriter.Step($"GetReservationsByCustomer({ids.CustomerId})");

        var booked = await reservations.GetReservationsByCustomerAsync(ids.CustomerId, cancellationToken);

        ConsoleWriter.Table(
            ["ReservationId", "Date", "PartySize", "Restaurant", "Table seats"],
            booked.Select(reservation => new[]
            {
                reservation.ReservationId.ToString(),
                reservation.ReservationDate.ToString("yyyy-MM-dd HH:mm"),
                reservation.PartySize.ToString(),
                reservation.Restaurant.Name,
                reservation.Table.Capacity.ToString()
            }));

        ConsoleWriter.Step($"ListOrdersAndMenuItems({ids.ReservationId})");

        var placed = await orders.ListOrdersAndMenuItemsAsync(ids.ReservationId, cancellationToken);

        if (placed.Count == 0)
        {
            ConsoleWriter.Info("(no orders)");
        }

        foreach (var order in placed)
        {
            ConsoleWriter.Detail($"Order #{order.OrderId} on {order.OrderDate:yyyy-MM-dd HH:mm}, total {order.TotalAmount:N2}");

            ConsoleWriter.Table(
                ["ItemId", "Menu item", "Unit price", "Qty", "Line total"],
                order.OrderItems.Select(item => new[]
                {
                    item.ItemId.ToString(),
                    item.MenuItem.Name,
                    item.MenuItem.Price.ToString("N2"),
                    item.Quantity.ToString(),
                    (item.MenuItem.Price * item.Quantity).ToString("N2")
                }));
        }

        ConsoleWriter.Step($"ListOrderedMenuItems({ids.ReservationId})");

        var ordered = await orders.ListOrderedMenuItemsAsync(ids.ReservationId, cancellationToken);

        ConsoleWriter.Table(
            ["ItemId", "Name", "Description", "Price"],
            ordered.Select(menuItem => new[]
            {
                menuItem.ItemId.ToString(),
                menuItem.Name,
                menuItem.Description ?? string.Empty,
                menuItem.Price.ToString("N2")
            }));

        ConsoleWriter.Step($"CalculateAverageOrderAmount({ids.EmployeeId})");

        var average = await employees.CalculateAverageOrderAmountAsync(ids.EmployeeId, cancellationToken);

        ConsoleWriter.Value($"average order amount for employee {ids.EmployeeId}", average.ToString("N2"));

        if (ids.EmployeeWithoutOrdersId is not { } idleEmployeeId)
        {
            ConsoleWriter.Info("every employee has at least one order, so the empty case cannot be shown");

            return;
        }

        ConsoleWriter.Step($"CalculateAverageOrderAmount({idleEmployeeId}) - employee with no orders");

        var noOrders = await employees.CalculateAverageOrderAmountAsync(idleEmployeeId, cancellationToken);

        ConsoleWriter.Value($"average order amount for employee {idleEmployeeId}", noOrders.ToString("N2"));
    }
}
