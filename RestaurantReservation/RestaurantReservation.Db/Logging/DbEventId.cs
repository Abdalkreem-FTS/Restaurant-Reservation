namespace RestaurantReservation.Db.Logging;

// Values are assigned by the compiler; only ever append, never insert or delete.
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
