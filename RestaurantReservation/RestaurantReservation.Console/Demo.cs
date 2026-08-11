namespace RestaurantReservation;

/// <summary>
/// Calls every repository method against the seeded database and prints what came back. Each section
/// takes its own scope, the way a request would.
/// </summary>
public sealed class Demo(IServiceScopeFactory scopes)
{
    public async Task RunAsync(CancellationToken cancellationToken)
    {
        var ids = await LoadSampleIdsAsync(cancellationToken);

        await CrudAsync(cancellationToken);
        await RestaurantsAsync(cancellationToken);
        await CustomersAsync(ids, cancellationToken);
        await EmployeesAsync(ids, cancellationToken);
        await ReservationsAsync(ids, cancellationToken);
        await OrdersAsync(ids, cancellationToken);
        await ErrorsAsync(ids, cancellationToken);
    }

    /// <summary>The shared <see cref="IRepository{TEntity}" /> contract, on customers.</summary>
    private async Task CrudAsync(CancellationToken cancellationToken)
    {
        Output.Title("CRUD and the unit of work");

        using var scope = scopes.CreateScope();
        var customers = scope.ServiceProvider.GetRequiredService<ICustomerRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var customer = new Customer
        {
            FirstName = "Ada",
            LastName = "Lovelace",
            Email = $"ada.{Guid.NewGuid():N}@example.com",
            PhoneNumber = "555-0199"
        };

        Output.Step("AddAsync(customer)");
        Output.Show(await customers.AddAsync(customer, cancellationToken), added => $"staged {added.FirstName} {added.LastName}, still unwritten so CustomerId is {added.CustomerId}");

        Output.Step("SaveChangesAsync()");
        Output.Show(await unitOfWork.SaveChangesAsync(cancellationToken), rows => $"{rows} row(s) written, CustomerId is now {customer.CustomerId}");

        Output.Step($"GetByIdAsync({customer.CustomerId})");
        Output.Show(await customers.GetByIdAsync(customer.CustomerId, cancellationToken), found => $"{found.FirstName} {found.LastName} <{found.Email}>");

        customer.LastName = "Byron";

        Output.Step("Update(customer) then SaveChangesAsync()");
        Output.Show(customers.Update(customer), _ => "staged the new surname");
        Output.Show(await unitOfWork.SaveChangesAsync(cancellationToken), rows => $"{rows} row(s) written");

        Output.Step("FindAsync(c => c.LastName == \"Byron\", page 1 of 5)");
        var matches = await customers.FindAsync(c => c.LastName == "Byron", new PageRequest { Size = 5 }, cancellationToken);
        Output.Table(["CustomerId", "Name", "Email"], matches.Items.Select(c => new[] { c.CustomerId.ToString(), $"{c.FirstName} {c.LastName}", c.Email }));

        Output.Step("GetAllAsync(page 1 of 3)");
        var page = await customers.GetAllAsync(new PageRequest { Number = 1, Size = 3 }, cancellationToken);
        Output.Note($"page {page.PageNumber} of {page.TotalPages}, {page.TotalCount} customer(s) in total, HasNext is {page.HasNext}");
        Output.Table(["CustomerId", "Name", "Email"], page.Items.Select(c => new[] { c.CustomerId.ToString(), $"{c.FirstName} {c.LastName}", c.Email }));

        Output.Step($"DeleteAsync({customer.CustomerId}) then SaveChangesAsync()");
        Output.Show(await customers.DeleteAsync(customer.CustomerId, cancellationToken), _ => "staged the removal");
        Output.Show(await unitOfWork.SaveChangesAsync(cancellationToken), rows => $"{rows} row(s) written");
        Output.Note("the demo customer is gone again, so the database is back to how it started");
    }

    /// <summary>The scalar function, <c>fn_CalculateRestaurantRevenue</c>.</summary>
    private async Task RestaurantsAsync(CancellationToken cancellationToken)
    {
        Output.Title("Restaurants - revenue from a database function");

        using var scope = scopes.CreateScope();
        var restaurants = scope.ServiceProvider.GetRequiredService<IRestaurantRepository>();

        Output.Step("GetTotalRevenueAsync(restaurantId) for every restaurant");

        var page = await restaurants.GetAllAsync(new PageRequest { Size = 10 }, cancellationToken);
        var rows = new List<string[]>();

        foreach (var restaurant in page.Items)
        {
            var revenue = await restaurants.GetTotalRevenueAsync(restaurant.RestaurantId, cancellationToken);

            rows.Add([
                restaurant.RestaurantId.ToString(),
                restaurant.Name,
                revenue.IsSuccess ? revenue.Value.ToString("N2") : revenue.TopError.Code
            ]);
        }

        Output.Table(["RestaurantId", "Restaurant", "Total revenue"], rows);
    }

    /// <summary>The stored procedure, <c>sp_FindCustomersByPartySize</c>.</summary>
    private async Task CustomersAsync(SampleIds ids, CancellationToken cancellationToken)
    {
        Output.Title("Customers - a stored procedure that pages itself");

        using var scope = scopes.CreateScope();
        var customers = scope.ServiceProvider.GetRequiredService<ICustomerRepository>();

        Output.Step($"FindCustomersByPartySizeAsync({ids.SmallestPartySize}, page 1 of 5)");

        var page = await customers.FindCustomersByPartySizeAsync(ids.SmallestPartySize, new PageRequest { Size = 5 }, cancellationToken);

        Output.Note($"{page.TotalCount} customer(s) have booked for more than {ids.SmallestPartySize}, showing page {page.PageNumber} of {page.TotalPages}");
        Output.Table(["CustomerId", "Name", "Email"], page.Items.Select(c => new[] { c.CustomerId.ToString(), $"{c.FirstName} {c.LastName}", c.Email }));
    }

    private async Task EmployeesAsync(SampleIds ids, CancellationToken cancellationToken)
    {
        Output.Title("Employees - filtering, aggregates and a view");

        using var scope = scopes.CreateScope();
        var employees = scope.ServiceProvider.GetRequiredService<IEmployeeRepository>();

        Output.Step("ListManagersAsync(page 1 of 10)");
        var managers = await employees.ListManagersAsync(new PageRequest { Size = 10 }, cancellationToken);
        Output.Table(
            ["EmployeeId", "Name", "Position", "RestaurantId"],
            managers.Items.Select(e => new[] { e.EmployeeId.ToString(), $"{e.FirstName} {e.LastName}", e.Position.ToString(), e.RestaurantId.ToString() }));

        Output.Step($"GetOrderAmountStatisticsAsync({ids.EmployeeId})");
        var statistics = await employees.GetOrderAmountStatisticsAsync(ids.EmployeeId, cancellationToken);
        Output.Value("orders", statistics.Count.ToString());
        Output.Value("sum", statistics.Sum.ToString("N2"));
        Output.Value("average", statistics.Average.ToString("N2"));
        Output.Value("min / max", $"{statistics.Min:N2} / {statistics.Max:N2}");
        Output.Value("variance", statistics.Variance.ToString("N2"));

        Output.Step("GetEmployeesWithRestaurantAsync(page 1 of 5) - reads vw_EmployeeDetails");
        var details = await employees.GetEmployeesWithRestaurantAsync(new PageRequest { Size = 5 }, cancellationToken);
        Output.Table(
            ["EmployeeId", "Name", "Position", "Restaurant"],
            details.Items.Select(e => new[] { e.EmployeeId.ToString(), $"{e.FirstName} {e.LastName}", e.Position, e.RestaurantName }));
    }

    private async Task ReservationsAsync(SampleIds ids, CancellationToken cancellationToken)
    {
        Output.Title("Reservations - navigations and a view");

        using var scope = scopes.CreateScope();
        var reservations = scope.ServiceProvider.GetRequiredService<IReservationRepository>();

        Output.Step($"GetReservationsByCustomerAsync({ids.CustomerId}, page 1 of 5)");
        var booked = await reservations.GetReservationsByCustomerAsync(ids.CustomerId, new PageRequest { Size = 5 }, cancellationToken);
        Output.Table(
            ["ReservationId", "When", "Party", "Restaurant", "Seats"],
            booked.Items.Select(r => new[]
            {
                r.ReservationId.ToString(),
                r.ReservationDate.ToString("yyyy-MM-dd HH:mm"),
                r.PartySize.ToString(),
                r.Restaurant.Name,
                r.Table.Capacity.ToString()
            }));

        Output.Step("GetReservationDetailsAsync(page 1 of 5) - reads vw_ReservationDetails");
        var details = await reservations.GetReservationDetailsAsync(new PageRequest { Size = 5 }, cancellationToken);
        Output.Table(
            ["ReservationId", "When", "Party", "Customer", "Restaurant"],
            details.Items.Select(r => new[]
            {
                r.ReservationId.ToString(),
                r.ReservationDate.ToString("yyyy-MM-dd HH:mm"),
                r.PartySize.ToString(),
                $"{r.CustomerFirstName} {r.CustomerLastName}",
                r.RestaurantName
            }));
    }

    private async Task OrdersAsync(SampleIds ids, CancellationToken cancellationToken)
    {
        Output.Title("Orders - include chains");

        using var scope = scopes.CreateScope();
        var orders = scope.ServiceProvider.GetRequiredService<IOrderRepository>();

        Output.Step($"ListOrdersAndMenuItemsAsync({ids.ReservationId}, page 1 of 5)");
        var placed = await orders.ListOrdersAndMenuItemsAsync(ids.ReservationId, new PageRequest { Size = 5 }, cancellationToken);

        foreach (var order in placed.Items)
        {
            Output.Value($"order {order.OrderId}", $"{order.OrderDate:yyyy-MM-dd HH:mm}, total {order.TotalAmount:N2}");
            Output.Table(
                ["ItemId", "Menu item", "Unit price", "Qty", "Line total"],
                order.OrderItems.Select(item => new[]
                {
                    item.ItemId.ToString(),
                    item.MenuItem.Name,
                    item.UnitPrice.ToString("N2"),
                    item.Quantity.ToString(),
                    (item.UnitPrice * item.Quantity).ToString("N2")
                }));
        }

        Output.Step($"ListOrderedMenuItemsAsync({ids.ReservationId}, page 1 of 5) - each item once");
        var ordered = await orders.ListOrderedMenuItemsAsync(ids.ReservationId, new PageRequest { Size = 5 }, cancellationToken);
        Output.Table(
            ["ItemId", "Name", "Price"],
            ordered.Items.Select(m => new[] { m.ItemId.ToString(), m.Name, m.Price.ToString("N2") }));
    }

    /// <summary>The failures callers are expected to handle, none of which throw.</summary>
    private async Task ErrorsAsync(SampleIds ids, CancellationToken cancellationToken)
    {
        Output.Title("Failures come back as results, not exceptions");

        using var scope = scopes.CreateScope();
        var customers = scope.ServiceProvider.GetRequiredService<ICustomerRepository>();
        var tables = scope.ServiceProvider.GetRequiredService<ITableRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        Output.Step("GetByIdAsync(-1) - a customer that is not there");
        Output.Show(await customers.GetByIdAsync(-1, cancellationToken), found => found.Email);

        Output.Step("DeleteAsync(-1) - removing something that is not there");
        Output.Show(await customers.DeleteAsync(-1, cancellationToken), _ => "removed");

        Output.Step("AddAsync(null) - an argument the repository rejects");
        Output.Show(await customers.AddAsync(null, cancellationToken), added => added.Email);

        Output.Step("SaveChangesAsync() for a table seating zero guests - a rule the database enforces");
        await tables.AddAsync(new Table { RestaurantId = ids.RestaurantId, Capacity = 0 }, cancellationToken);
        Output.Show(await unitOfWork.SaveChangesAsync(cancellationToken), rows => $"{rows} row(s) written");
        Output.Note("the check constraint rejected it, and the translator turned that into the message above");
    }

    private async Task<SampleIds> LoadSampleIdsAsync(CancellationToken cancellationToken)
    {
        using var scope = scopes.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<RestaurantReservationDbContext>();

        var reservation = await context.Reservations
            .Include(r => r.Orders)
            .OrderBy(r => r.ReservationId)
            .FirstOrDefaultAsync(r => r.Orders.Any(), cancellationToken)
            ?? throw new InvalidOperationException(
                "No reservation with an order was found, so there is nothing to demonstrate. Apply the migrations to create the schema and its seed data.");

        return new SampleIds(
            RestaurantId: reservation.RestaurantId,
            CustomerId: reservation.CustomerId,
            EmployeeId: reservation.Orders.First().EmployeeId,
            ReservationId: reservation.ReservationId,
            SmallestPartySize: await context.Reservations.MinAsync(r => r.PartySize, cancellationToken));
    }

    private sealed record SampleIds(int RestaurantId, int CustomerId, int EmployeeId, int ReservationId, int SmallestPartySize);
}
