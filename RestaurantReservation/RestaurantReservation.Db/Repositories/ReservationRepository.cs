using RestaurantReservation.Db.Abstractions;
using RestaurantReservation.Db.Entities;
using RestaurantReservation.Db.Entities.Views;
using RestaurantReservation.Db.Pagination;

namespace RestaurantReservation.Db.Repositories;

public class ReservationRepository(RestaurantReservationDbContext context) : Repository<Reservation>(context), IReservationRepository
{
    public async Task<PagedResult<Reservation>> GetReservationsByCustomerAsync(int customerId, PageRequest page, CancellationToken cancellationToken = default)
    {
        return await Context.Reservations
            .Where(r => r.CustomerId == customerId)
            .Include(r => r.Restaurant)
            .Include(r => r.Table)
            .AsNoTracking()
            .OrderByDescending(r => r.ReservationDate)
            .ThenBy(r => r.ReservationId)
            .GetPageAsync(page, cancellationToken);
    }

    public async Task<PagedResult<ReservationDetail>> GetReservationDetailsAsync(PageRequest page, CancellationToken cancellationToken = default)
    {
        return await Context.ReservationDetails
            .OrderBy(r => r.ReservationId)
            .GetPageAsync(page, cancellationToken);
    }
}
