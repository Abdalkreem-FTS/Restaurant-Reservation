using Microsoft.Extensions.Logging;
using RestaurantReservation.Db.Results;

namespace RestaurantReservation.Db.Logging;

internal static class DbLogLevelPolicy
{
    public static LogLevel For(ErrorType type) => type switch
    {
        ErrorType.Validation or ErrorType.Conflict or ErrorType.NotFound => LogLevel.Information,
        ErrorType.Timeout or ErrorType.Unavailable => LogLevel.Warning,
        _ => LogLevel.Error,
    };
}
