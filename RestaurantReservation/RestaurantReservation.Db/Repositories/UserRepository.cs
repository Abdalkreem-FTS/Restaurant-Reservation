using Microsoft.Extensions.Logging;
using RestaurantReservation.Db.Abstractions;
using RestaurantReservation.Db.Entities;
using RestaurantReservation.Db.Logging;
using RestaurantReservation.Db.Results;

namespace RestaurantReservation.Db.Repositories;

public class UserRepository(RestaurantReservationDbContext context, ILogger<UserRepository> logger) : Repository<User>(context, logger), IUserRepository
{
    public async Task<Result<User>> FindByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        var user = await Context.Users
            .Include(u => u.Customer)
            .Include(u => u.Employee)
            .AsNoTracking()
            .SingleOrDefaultAsync(u => u.Username == username, cancellationToken);

        if (user is not null)
        {
            return user;
        }

        Logger.LogInformation(DbEvents.EntityNotFound, "User {Username} was not found", username);

        return Error.NotFound("Users.NotFound", $"User '{username}' was not found.");
    }
}
