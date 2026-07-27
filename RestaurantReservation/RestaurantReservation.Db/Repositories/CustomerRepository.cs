using RestaurantReservation.Db.Abstractions;
using RestaurantReservation.Db.Entities;
using RestaurantReservation.Db.Pagination;

namespace RestaurantReservation.Db.Repositories;

public class CustomerRepository(RestaurantReservationDbContext context) : Repository<Customer>(context), ICustomerRepository
{
    public async Task<PagedResult<Customer>> FindCustomersByPartySizeAsync(int partySize, PageRequest page, CancellationToken cancellationToken = default)
    {
        var totalCount = await Context.Customers
            .CountAsync(c => c.Reservations.Any(r => r.PartySize > partySize), cancellationToken);

        var pagedResult = await Context.Customers
            .FromSqlInterpolated(
                $"EXEC dbo.sp_FindCustomersByPartySize @PartySize = {partySize}")
            .GetPageAsync(page, cancellationToken);

        return new PagedResult<Customer>(pagedResult.Items, totalCount, page.Number, page.Size);
    }
}
