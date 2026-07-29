namespace RestaurantReservation.Db.Results;

/// <summary>
/// A failed database call described in full: the <see cref="Error" /> the caller is given, plus the
/// store's own identifiers for it. <see cref="DbExceptionTranslator.Translate" /> keeps only the
/// error, which is all a caller can act on; a log entry wants the rest so a failure can be traced
/// back to the exact constraint or SQL Server error that caused it.
/// </summary>
/// <param name="Error">The domain error the failure was translated into.</param>
/// <param name="SqlErrorNumber">SQL Server's error number, or null when the failure did not come from the store.</param>
/// <param name="ConstraintName">The constraint that rejected the statement, or null when no constraint was named.</param>
public readonly record struct DatabaseFailure(Error Error, int? SqlErrorNumber, string? ConstraintName);
