using RestaurantReservation.Db.Abstractions;
using RestaurantReservation.Db.Entities;
using RestaurantReservation.Db.Entities.Views;

namespace RestaurantReservation.Db.Repositories;

public class ReservationRepository(RestaurantReservationDbContext context) : Repository<Reservation>(context), IReservationRepository
{
    public async Task<IReadOnlyList<Reservation>> GetReservationsByCustomerAsync(int customerId, CancellationToken cancellationToken = default)
    {
        return await Context.Reservations
            .Where(r => r.CustomerId == customerId)
            .Include(r => r.Restaurant)
            .Include(r => r.Table)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ReservationDetail>> GetReservationDetailsAsync(CancellationToken cancellationToken = default)
    {
        return await Context.ReservationDetails.ToListAsync(cancellationToken);
    }
}