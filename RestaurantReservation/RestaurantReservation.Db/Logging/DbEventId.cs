namespace RestaurantReservation.Db.Logging;

internal enum DbEventId
{
    SaveSucceeded = 1000,
    SaveFailed,
    EntityNotFound,
    NullEntityRejected,
    CommandFailed,
    CommandSlow,
    TransactionStarted,
    TransactionCommitted,
    TransactionRolledBack,
    TransactionFailed,
}
