using Microsoft.Extensions.Logging;
using RestaurantReservation.Db.Results;

namespace RestaurantReservation.Db.Logging;

internal static partial class UnitOfWorkLog
{
    [LoggerMessage(EventId = (int)DbEventId.SaveSucceeded, Level = LogLevel.Debug, Message = "Saved {RowsAffected} change(s)")]
    public static partial void SaveSucceeded(ILogger logger, int rowsAffected);
    
    [LoggerMessage(EventId = (int)DbEventId.SaveFailed, Message = "Save failed with {ErrorCode} ({ErrorType}), SQL error {SqlErrorNumber}, constraint {ConstraintName}")]
    public static partial void SaveFailed(
        ILogger logger,
        LogLevel level,
        string errorCode,
        ErrorType errorType,
        int? sqlErrorNumber,
        string? constraintName,
        Exception? exception);
}
