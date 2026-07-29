using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using RestaurantReservation.Db.Results;

namespace RestaurantReservation.Db.Logging;

internal sealed class TransactionLoggingInterceptor(ILogger logger) : DbTransactionInterceptor
{
    public override DbTransaction TransactionStarted(
        DbConnection connection,
        TransactionEndEventData eventData,
        DbTransaction result)
    {
        TransactionLog.TransactionStarted(logger, eventData.TransactionId);

        return base.TransactionStarted(connection, eventData, result);
    }

    public override ValueTask<DbTransaction> TransactionStartedAsync(
        DbConnection connection,
        TransactionEndEventData eventData,
        DbTransaction result,
        CancellationToken cancellationToken = default)
    {
        TransactionLog.TransactionStarted(logger, eventData.TransactionId);

        return base.TransactionStartedAsync(connection, eventData, result, cancellationToken);
    }

    public override void TransactionCommitted(DbTransaction transaction, TransactionEndEventData eventData)
    {
        LogCommitted(eventData);

        base.TransactionCommitted(transaction, eventData);
    }

    public override Task TransactionCommittedAsync(
        DbTransaction transaction,
        TransactionEndEventData eventData,
        CancellationToken cancellationToken = default)
    {
        LogCommitted(eventData);

        return base.TransactionCommittedAsync(transaction, eventData, cancellationToken);
    }

    public override void TransactionRolledBack(DbTransaction transaction, TransactionEndEventData eventData)
    {
        LogRolledBack(eventData);

        base.TransactionRolledBack(transaction, eventData);
    }

    public override Task TransactionRolledBackAsync(
        DbTransaction transaction,
        TransactionEndEventData eventData,
        CancellationToken cancellationToken = default)
    {
        LogRolledBack(eventData);

        return base.TransactionRolledBackAsync(transaction, eventData, cancellationToken);
    }

    public override void TransactionFailed(DbTransaction transaction, TransactionErrorEventData eventData)
    {
        LogFailure(eventData);

        base.TransactionFailed(transaction, eventData);
    }

    public override Task TransactionFailedAsync(
        DbTransaction transaction,
        TransactionErrorEventData eventData,
        CancellationToken cancellationToken = default)
    {
        LogFailure(eventData);

        return base.TransactionFailedAsync(transaction, eventData, cancellationToken);
    }

    private void LogCommitted(TransactionEndEventData eventData) =>
        TransactionLog.TransactionCommitted(logger, eventData.TransactionId, (long)eventData.Duration.TotalMilliseconds);

    private void LogRolledBack(TransactionEndEventData eventData) =>
        TransactionLog.TransactionRolledBack(logger, eventData.TransactionId, (long)eventData.Duration.TotalMilliseconds);
    
    private void LogFailure(TransactionErrorEventData eventData)
    {
        var error = DbExceptionTranslator.Translate(eventData.Exception);
        var level = DbLogLevelPolicy.For(error.Type);

        TransactionLog.TransactionFailed(
            logger,
            level,
            eventData.TransactionId,
            eventData.Action,
            error.Code,
            level == LogLevel.Error ? eventData.Exception : null);
    }
}
