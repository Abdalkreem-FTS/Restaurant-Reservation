namespace RestaurantReservation;

public sealed record SampleIds(
    int RestaurantId,
    int CustomerId,
    int TableId,
    int EmployeeId,
    int MenuItemId,
    int ReservationId,
    int OrderId,
    int? EmployeeWithoutOrdersId,
    int SmallestPartySize,
    int LargestPartySize)
{
    public static async Task<SampleIds> LoadAsync(RestaurantReservationDbContext context, CancellationToken cancellationToken)
    {
        var reservation = await context.Reservations
            .Include(r => r.Orders)
                .ThenInclude(o => o.OrderItems)
            .OrderBy(r => r.ReservationId)
            .FirstOrDefaultAsync(r => r.Orders.Any(o => o.OrderItems.Count > 0), cancellationToken)
            ?? throw new InvalidOperationException(
                "No reservation with an order was found, so there is nothing to demonstrate. Apply the " +
                "migrations (dotnet ef database update, or dotnet run -- --migrate) to create the schema " +
                "and its seed data.");

        var order = reservation.Orders.First(o => o.OrderItems.Count > 0);

        return new SampleIds(
            RestaurantId: reservation.RestaurantId,
            CustomerId: reservation.CustomerId,
            TableId: reservation.TableId,
            EmployeeId: order.EmployeeId,
            MenuItemId: order.OrderItems.First().ItemId,
            ReservationId: reservation.ReservationId,
            OrderId: order.OrderId,
            EmployeeWithoutOrdersId: await context.Employees
                .Where(e => !e.Orders.Any())
                .Select(e => (int?)e.EmployeeId)
                .FirstOrDefaultAsync(cancellationToken),
            SmallestPartySize: await context.Reservations.MinAsync(r => r.PartySize, cancellationToken),
            LargestPartySize: await context.Reservations.MaxAsync(r => r.PartySize, cancellationToken));
    }
}
