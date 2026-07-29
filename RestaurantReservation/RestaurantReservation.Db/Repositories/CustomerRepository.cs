using Microsoft.Extensions.Logging;
using RestaurantReservation.Db.Abstractions;
using RestaurantReservation.Db.Entities;
using RestaurantReservation.Db.Pagination;

namespace RestaurantReservation.Db.Repositories;

public class CustomerRepository(RestaurantReservationDbContext context, ILogger<CustomerRepository> logger) : Repository<Customer>(context, logger), ICustomerRepository
{
    public async Task<PagedResult<Customer>> FindCustomersByPartySizeAsync(int partySize, PageRequest page, CancellationToken cancellationToken = default)
    {
        var totalCount = await Context.Customers
            .CountAsync(c => c.Reservations.Any(r => r.PartySize > partySize), cancellationToken);

        var items = await Context.Customers
            .FromSqlInterpolated(
                $"EXEC dbo.sp_FindCustomersByPartySize @PartySize = {partySize}, @Offset = {page.Skip}, @PageSize = {page.Size}")
            .ToListAsync(cancellationToken);

        return new PagedResult<Customer>(items, totalCount, page.Number, page.Size);
    }
}
