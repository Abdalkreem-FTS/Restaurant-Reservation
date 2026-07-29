using Microsoft.Extensions.Logging;

namespace RestaurantReservation.Db.Logging;

internal static partial class TransactionLog
{
    [LoggerMessage(EventId = (int)DbEventId.TransactionStarted, Level = LogLevel.Debug, Message = "Transaction {TransactionId} started")]
    public static partial void TransactionStarted(ILogger logger, Guid transactionId);

    [LoggerMessage(EventId = (int)DbEventId.TransactionCommitted, Level = LogLevel.Debug, Message = "Transaction {TransactionId} committed after {ElapsedMs} ms")]
    public static partial void TransactionCommitted(ILogger logger, Guid transactionId, long elapsedMs);
    
    [LoggerMessage(EventId = (int)DbEventId.TransactionRolledBack, Level = LogLevel.Debug, Message = "Transaction {TransactionId} rolled back after {ElapsedMs} ms")]
    public static partial void TransactionRolledBack(ILogger logger, Guid transactionId, long elapsedMs);

    [LoggerMessage(EventId = (int)DbEventId.TransactionFailed, Message = "Transaction {TransactionId} failed during {Action} with {ErrorCode}")]
    public static partial void TransactionFailed(
        ILogger logger,
        LogLevel level,
        Guid transactionId,
        string action,
        string errorCode,
        Exception? exception);
}
