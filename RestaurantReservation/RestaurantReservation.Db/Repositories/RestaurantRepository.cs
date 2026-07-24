using RestaurantReservation.Db.Abstractions;
using RestaurantReservation.Db.Entities;

namespace RestaurantReservation.Db.Repositories;

public class RestaurantRepository(RestaurantReservationDbContext context) : Repository<Restaurant>(context), IRestaurantRepository
{
    public async Task<decimal> GetTotalRevenueAsync(int restaurantId, CancellationToken cancellationToken = default)
    {
        return await Context.Restaurants
            .Where(r => r.RestaurantId == restaurantId)
            .Select(r => RestaurantReservationDbContext.CalculateRestaurantRevenue(r.RestaurantId))
            .FirstOrDefaultAsync(cancellationToken);
    }
}