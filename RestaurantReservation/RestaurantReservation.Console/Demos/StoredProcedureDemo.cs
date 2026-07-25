namespace RestaurantReservation.Demos;

public sealed class StoredProcedureDemo(Func<RestaurantReservationDbContext> newContext)
{
    public async Task RunAsync(SampleIds ids, CancellationToken cancellationToken)
    {
        await using var context = newContext();

        var customers = new CustomerRepository(context);

        int[] thresholds = [ids.SmallestPartySize - 1, ids.SmallestPartySize, ids.LargestPartySize - 1];

        foreach (var threshold in thresholds.Distinct().Where(threshold => threshold >= 0))
        {
            ConsoleWriter.Step($"FindCustomersByPartySize({threshold})");

            var found = await customers.FindCustomersByPartySizeAsync(threshold, cancellationToken);

            ConsoleWriter.Table(
                ["CustomerId", "Name", "Email", "Phone"],
                found.Select(customer => new[]
                {
                    customer.CustomerId.ToString(),
                    $"{customer.FirstName} {customer.LastName}",
                    customer.Email,
                    customer.PhoneNumber
                }));
        }
    }
}
