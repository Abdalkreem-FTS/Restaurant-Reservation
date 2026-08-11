using RestaurantReservation.Db.Entities;
using RestaurantReservation.Db.Results;

namespace RestaurantReservation.Db.Abstractions;

public interface IUserRepository : IRepository<User>
{
    Task<Result<User>> FindByUsernameAsync(string username, CancellationToken cancellationToken = default);
}
