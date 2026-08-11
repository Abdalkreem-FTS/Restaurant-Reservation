using RestaurantReservation.Db.Entities;
using RestaurantReservation.Db.Pagination;

namespace RestaurantReservation.Db.Abstractions;

public interface ICustomerRepository : IRepository<Customer>
{
    /// <summary>
    /// Returns a page of customers who have made at least one reservation with a party size greater than
    /// <paramref name="partySize" />. Executes the <c>dbo.sp_FindCustomersByPartySize</c> stored procedure.
    /// </summary>
    Task<PagedResult<Customer>> FindCustomersByPartySizeAsync(int partySize, PageRequest page, CancellationToken cancellationToken = default);
}
