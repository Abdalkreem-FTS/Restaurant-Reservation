using Microsoft.Extensions.Logging;
using RestaurantReservation.Db.Abstractions;
using RestaurantReservation.Db.Entities;
using RestaurantReservation.Db.Results;

namespace RestaurantReservation.Db.Repositories;

public class RestaurantRepository(RestaurantReservationDbContext context, ILogger<RestaurantRepository> logger) : Repository<Restaurant>(context, logger), IRestaurantRepository
{
    public async Task<Result<decimal>> GetTotalRevenueAsync(int restaurantId, CancellationToken cancellationToken = default)
    {
        var revenue = await Context.Restaurants
            .Where(r => r.RestaurantId == restaurantId)
            .Select(r => (decimal?)RestaurantReservationDbContext.CalculateRestaurantRevenue(r.RestaurantId))
            .FirstOrDefaultAsync(cancellationToken);

        return revenue is not null
            ? revenue.Value
            : Error.NotFound("Restaurants.NotFound", $"Restaurant with id {restaurantId} was not found.");
    }
}
