using RestaurantReservation.Db.Abstractions;
using RestaurantReservation.Db.Entities;

namespace RestaurantReservation.Db.Repositories;

public class CustomerRepository(RestaurantReservationDbContext context) : Repository<Customer>(context), ICustomerRepository
{
    public async Task<IReadOnlyList<Customer>> FindCustomersByPartySizeAsync(int partySize, CancellationToken cancellationToken = default)
    {
        return await Context.Customers
            .FromSqlInterpolated($"EXEC dbo.sp_FindCustomersByPartySize @PartySize = {partySize}")
            .ToListAsync(cancellationToken);
    }
}