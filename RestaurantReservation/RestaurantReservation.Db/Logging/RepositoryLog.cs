using Microsoft.Extensions.Logging;

namespace RestaurantReservation.Db.Logging;

internal static partial class RepositoryLog
{
    [LoggerMessage(EventId = (int)DbEventId.EntityNotFound, Level = LogLevel.Information, Message = "{EntityType} {EntityId} was not found")]
    public static partial void EntityNotFound(ILogger logger, string entityType, int entityId);

    [LoggerMessage(EventId = (int)DbEventId.NullEntityRejected, Level = LogLevel.Information, Message = "A null {EntityType} was rejected by {Operation}")]
    public static partial void NullEntityRejected(ILogger logger, string entityType, string operation);
}
