using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using RestaurantReservation.Db.Results;

namespace RestaurantReservation.Db.Logging;

internal sealed class CommandLoggingInterceptor(ILogger logger, TimeSpan slowCommandThreshold) : DbCommandInterceptor
{
    public override DbDataReader ReaderExecuted(DbCommand command, CommandExecutedEventData eventData, DbDataReader result)
    {
        WarnIfSlow(eventData);

        return base.ReaderExecuted(command, eventData, result);
    }

    public override ValueTask<DbDataReader> ReaderExecutedAsync(
        DbCommand command,
        CommandExecutedEventData eventData,
        DbDataReader result,
        CancellationToken cancellationToken = default)
    {
        WarnIfSlow(eventData);

        return base.ReaderExecutedAsync(command, eventData, result, cancellationToken);
    }

    public override object? ScalarExecuted(DbCommand command, CommandExecutedEventData eventData, object? result)
    {
        WarnIfSlow(eventData);

        return base.ScalarExecuted(command, eventData, result);
    }

    public override ValueTask<object?> ScalarExecutedAsync(
        DbCommand command,
        CommandExecutedEventData eventData,
        object? result,
        CancellationToken cancellationToken = default)
    {
        WarnIfSlow(eventData);

        return base.ScalarExecutedAsync(command, eventData, result, cancellationToken);
    }

    public override int NonQueryExecuted(DbCommand command, CommandExecutedEventData eventData, int result)
    {
        WarnIfSlow(eventData);

        return base.NonQueryExecuted(command, eventData, result);
    }

    public override ValueTask<int> NonQueryExecutedAsync(
        DbCommand command,
        CommandExecutedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        WarnIfSlow(eventData);

        return base.NonQueryExecutedAsync(command, eventData, result, cancellationToken);
    }

    public override void CommandFailed(DbCommand command, CommandErrorEventData eventData)
    {
        LogFailure(command, eventData);

        base.CommandFailed(command, eventData);
    }

    public override Task CommandFailedAsync(
        DbCommand command,
        CommandErrorEventData eventData,
        CancellationToken cancellationToken = default)
    {
        LogFailure(command, eventData);

        return base.CommandFailedAsync(command, eventData, cancellationToken);
    }

    private void WarnIfSlow(CommandExecutedEventData eventData)
    {
        if (eventData.Duration < slowCommandThreshold)
        {
            return;
        }

        logger.LogWarning(
            DbEvents.CommandSlow,
            "Database command took {ElapsedMs} ms, above the {ThresholdMs} ms threshold: {CommandText}",
            (long)eventData.Duration.TotalMilliseconds,
            (long)slowCommandThreshold.TotalMilliseconds,
            eventData.Command.CommandText);
    }

    private void LogFailure(DbCommand command, CommandErrorEventData eventData)
    {
        var failure = DbExceptionTranslator.Describe(eventData.Exception);
        var level = DbLogLevelPolicy.For(failure.Error.Type);

        // UnitOfWork logs expected save failures as SaveFailed; repeating them here would be noise.
        if (eventData.CommandSource == CommandSource.SaveChanges && level < LogLevel.Error)
        {
            return;
        }

        logger.Log(
            level,
            DbEvents.CommandFailed,
            level == LogLevel.Error ? eventData.Exception : null,
            "Database command failed after {ElapsedMs} ms with {ErrorCode} (SQL error {SqlErrorNumber}): {CommandText}",
            (long)eventData.Duration.TotalMilliseconds,
            failure.Error.Code,
            failure.SqlErrorNumber,
            command.CommandText);
    }
}
