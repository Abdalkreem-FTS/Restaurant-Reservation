namespace RestaurantReservation.Demos;

public sealed class FunctionDemo(Func<RestaurantReservationDbContext> newContext)
{
    public async Task RunAsync(SampleIds ids, CancellationToken cancellationToken)
    {
        await using var context = newContext();

        var repository = new RestaurantRepository(context);

        ConsoleWriter.Step("GetTotalRevenue(restaurantId) for every restaurant");

        var restaurants = await repository.GetAllAsync(cancellationToken);
        var rows = new List<string[]>();

        foreach (var restaurant in restaurants)
        {
            var revenue = await repository.GetTotalRevenueAsync(restaurant.RestaurantId, cancellationToken);

            rows.Add([restaurant.RestaurantId.ToString(), restaurant.Name, revenue.ToString("N2")]);
        }

        ConsoleWriter.Table(["RestaurantId", "Restaurant", "Total revenue"], rows);
    }
}
