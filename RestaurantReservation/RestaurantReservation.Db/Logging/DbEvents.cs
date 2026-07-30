using Microsoft.Extensions.Logging;

namespace RestaurantReservation.Db.Logging;

internal static class DbEvents
{
    public static readonly EventId SaveSucceeded = new(1000, nameof(SaveSucceeded));
    public static readonly EventId SaveFailed = new(1001, nameof(SaveFailed));
    public static readonly EventId EntityNotFound = new(1002, nameof(EntityNotFound));
    public static readonly EventId NullEntityRejected = new(1003, nameof(NullEntityRejected));
    public static readonly EventId CommandFailed = new(1004, nameof(CommandFailed));
    public static readonly EventId CommandSlow = new(1005, nameof(CommandSlow));
}
