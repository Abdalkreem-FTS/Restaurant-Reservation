using RestaurantReservation.Db.Entities;

namespace RestaurantReservation.Db.Abstractions;

public interface ICustomerRepository : IRepository<Customer>
{
    /// <summary>
    /// Returns all customers who have made at least one reservation with a party size greater than
    /// <paramref name="partySize" />. Executes the <c>dbo.sp_FindCustomersByPartySize</c> stored procedure.
    /// </summary>
    Task<IReadOnlyList<Customer>> FindCustomersByPartySizeAsync(int partySize, CancellationToken cancellationToken = default);
}