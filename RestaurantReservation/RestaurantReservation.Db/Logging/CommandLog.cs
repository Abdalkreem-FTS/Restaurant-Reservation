using Microsoft.Extensions.Logging;

namespace RestaurantReservation.Db.Logging;

internal static partial class CommandLog
{
    [LoggerMessage(EventId = (int)DbEventId.CommandFailed, Message = "Database command failed after {ElapsedMs} ms with {ErrorCode} (SQL error {SqlErrorNumber}): {CommandText}")]
    public static partial void CommandFailed(
        ILogger logger,
        LogLevel level,
        long elapsedMs,
        string errorCode,
        int? sqlErrorNumber,
        string commandText,
        Exception? exception);

    [LoggerMessage(EventId = (int)DbEventId.CommandSlow, Level = LogLevel.Warning, Message = "Database command took {ElapsedMs} ms, above the {ThresholdMs} ms threshold: {CommandText}")]
    public static partial void CommandSlow(ILogger logger, long elapsedMs, long thresholdMs, string commandText);
}
