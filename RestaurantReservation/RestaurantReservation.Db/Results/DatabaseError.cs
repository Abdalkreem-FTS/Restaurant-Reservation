namespace RestaurantReservation.Db.Results;

public static class DatabaseError
{
    public static Error ConcurrencyConflict => Error.Conflict(
        "Db.ConcurrencyConflict",
        "One or more records were changed or removed by someone else after they were read. Reload them and try again.");

    public static Error Deadlock => Error.Conflict(
        "Db.Deadlock",
        "The operation was chosen as a deadlock victim and was rolled back. Try again.");

    public static Error LockTimeout => Error.Conflict(
        "Db.LockTimeout",
        "The operation gave up waiting for another transaction to release a lock. Try again.");

    public static Error SnapshotConflict => Error.Conflict(
        "Db.SnapshotConflict",
        "Another transaction changed the same records at the same time. Try again.");

    public static Error Timeout => Error.Timeout(
        "Db.Timeout",
        "The database did not respond in time. The changes may or may not have been saved, so check before retrying.");

    public static Error Unavailable => Error.Unavailable(
        "Db.Unavailable",
        "The database could not be reached. Try again shortly.");

    public static Error LoginFailed => Error.Unavailable(
        "Db.LoginFailed",
        "The application could not sign in to the database.");

    public static Error Throttled => Error.Unavailable(
        "Db.Throttled",
        "The database is at capacity and is refusing new requests. Try again shortly.");

    public static Error IdentityInsertNotAllowed => Error.Failure(
        "Db.IdentityInsertNotAllowed",
        "An id was supplied for a column the database assigns itself. Leave the id unset when adding a new record.");

    public static Error SaveFailed => Error.Failure(
        "Db.SaveFailed",
        "The changes could not be saved.");

    public static Error ValueTooLong => Error.Validation(
        "Db.ValueTooLong",
        "One of the supplied values is longer than its column allows.");

    public static Error ValueOutOfRange => Error.Validation(
        "Db.ValueOutOfRange",
        "One of the supplied numbers is outside the range its column allows.");

    public static Error InvalidValueFormat => Error.Validation(
        "Db.InvalidValueFormat",
        "One of the supplied values is not in a format its column accepts.");

    public static Error RequiredValueMissing(string? column) => Error.Validation(
        "Db.RequiredValueMissing",
        column is null
            ? "A required value was not supplied."
            : $"'{column}' is required and was not supplied.");

    public static Error UnexpectedSqlError(int number) => Error.Unexpected(
        "Db.UnexpectedSqlError",
        $"The database reported an unexpected error (SQL error {number}).");

    public static Error Unexpected(Exception exception) => Error.Unexpected(
        "Db.Unexpected",
        $"An unexpected error occurred while accessing the database ({exception.GetType().Name}).");

    /// <summary>
    /// A unique index, primary key or alternate key rejected the write.
    /// </summary>
    public static Error DuplicateKey(string? constraint) => constraint switch
    {
        not null when _duplicateKeys.TryGetValue(constraint, out var known) => known,
        not null when HasPrefix(constraint, "PK_") => Error.Conflict(
            "Db.DuplicatePrimaryKey",
            $"A record with the same primary key already exists ({constraint})."),
        not null when HasPrefix(constraint, "AK_") => Error.Conflict(
            "Db.DuplicateAlternateKey",
            $"A record with the same key values already exists ({constraint})."),
        _ => Error.Conflict(
            "Db.DuplicateKey",
            "A record with the same unique values already exists."),
    };

    /// <summary>
    /// A check constraint rejected the value.
    /// </summary>
    public static Error CheckViolation(string? constraint) => Lookup(
        _checkViolations,
        constraint,
        Error.Validation(
            "Db.CheckConstraintViolated",
            "One of the supplied values breaks a rule the database enforces."));

    /// <summary>
    /// A foreign key rejected an insert or update because the record being pointed at does not exist.
    /// For the composite keys that carry <c>RestaurantId</c> this also fires when the record exists
    /// but belongs to a different restaurant, which is the whole point of those keys.
    /// </summary>
    public static Error MissingRelatedRecord(string? constraint) => Lookup(
        _missingRelatedRecords,
        constraint,
        Error.Validation(
            "Db.RelatedRecordMissing",
            "A record this one refers to does not exist, or belongs to a different restaurant."));

    /// <summary>
    /// A foreign key rejected a delete because rows still point at the record being removed.
    /// </summary>
    public static Error StillReferenced(string? constraint) => Lookup(
        _stillReferencedRecords,
        constraint,
        Error.Conflict(
            "Db.StillReferenced",
            "This record cannot be deleted because other records still refer to it."));

    private static readonly Dictionary<string, Error> _duplicateKeys = new(StringComparer.OrdinalIgnoreCase)
    {
        ["IX_Customers_Email"] = Error.Conflict(
            "Customers.DuplicateEmail",
            "A customer with this email address already exists."),

        ["IX_Reservations_TableId_ReservationDate"] = Error.Conflict(
            "Reservations.TableAlreadyBooked",
            "That table is already reserved for the selected date and time."),
    };

    private static readonly Dictionary<string, Error> _checkViolations = new(StringComparer.OrdinalIgnoreCase)
    {
        ["CK_Reservations_PartySizeIsPositive"] = Error.Validation(
            "Reservations.PartySizeIsNotPositive",
            "Party size must be at least one guest."),

        ["CK_Reservations_PartySizeWithinTableCapacity"] = Error.Validation(
            "Reservations.PartySizeExceedsTableCapacity",
            "Party size exceeds the capacity of the selected table."),

        ["CK_Reservations_ReservationDateOnTheHour"] = Error.Validation(
            "Reservations.ReservationDateNotOnTheHour",
            "Reservations can only start exactly on the hour."),

        ["CK_OrderItems_UnitPriceIsNotNegative"] = Error.Validation(
            "OrderItems.UnitPriceIsNegative",
            "Unit price cannot be negative."),

        ["CK_OrderItems_QuantityIsPositive"] = Error.Validation(
            "OrderItems.QuantityIsNotPositive",
            "An order item must be for at least one unit."),

        ["CK_MenuItems_PriceIsNotNegative"] = Error.Validation(
            "MenuItems.PriceIsNegative",
            "A menu item's price cannot be negative."),

        ["CK_Tables_CapacityIsPositive"] = Error.Validation(
            "Tables.CapacityIsNotPositive",
            "A table must seat at least one guest."),
    };

    private static readonly Dictionary<string, Error> _missingRelatedRecords = new(StringComparer.OrdinalIgnoreCase)
    {
        ["FK_Reservations_Customers_CustomerId"] = Error.Validation(
            "Reservations.CustomerNotFound",
            "The selected customer does not exist."),

        ["FK_Reservations_Restaurants_RestaurantId"] = Error.Validation(
            "Reservations.RestaurantNotFound",
            "The selected restaurant does not exist."),

        ["FK_Reservations_Tables_TableId_RestaurantId_TableCapacity"] = Error.Validation(
            "Reservations.TableNotAvailableAtRestaurant",
            "The selected table does not exist, belongs to a different restaurant, or its capacity no longer matches the one recorded on the reservation."),

        ["FK_Orders_Reservations_ReservationId_RestaurantId"] = Error.Validation(
            "Orders.ReservationNotAtRestaurant",
            "The selected reservation does not exist or belongs to a different restaurant."),

        ["FK_Orders_Employees_EmployeeId_RestaurantId"] = Error.Validation(
            "Orders.EmployeeNotAtRestaurant",
            "The selected employee does not exist or does not work at this restaurant."),

        ["FK_OrderItems_Orders_OrderId_RestaurantId"] = Error.Validation(
            "OrderItems.OrderNotAtRestaurant",
            "The selected order does not exist or belongs to a different restaurant."),

        ["FK_OrderItems_MenuItems_ItemId_RestaurantId"] = Error.Validation(
            "OrderItems.MenuItemNotAtRestaurant",
            "The selected menu item does not exist or is not on this restaurant's menu."),

        ["FK_Employees_Restaurants_RestaurantId"] = Error.Validation(
            "Employees.RestaurantNotFound",
            "The selected restaurant does not exist."),

        ["FK_MenuItems_Restaurants_RestaurantId"] = Error.Validation(
            "MenuItems.RestaurantNotFound",
            "The selected restaurant does not exist."),

        ["FK_Tables_Restaurants_RestaurantId"] = Error.Validation(
            "Tables.RestaurantNotFound",
            "The selected restaurant does not exist."),
    };

    /// <remarks>
    /// Only the foreign keys configured as <see cref="DeleteBehavior.Restrict" /> can appear here.
    /// <c>FK_OrderItems_Orders_OrderId_RestaurantId</c> is deliberately absent: it cascades, so
    /// deleting an order takes its items with it rather than being blocked by them.
    /// </remarks>
    private static readonly Dictionary<string, Error> _stillReferencedRecords = new(StringComparer.OrdinalIgnoreCase)
    {
        ["FK_Reservations_Customers_CustomerId"] = Error.Conflict(
            "Customers.HasReservations",
            "This customer cannot be deleted while they still have reservations."),

        ["FK_Reservations_Restaurants_RestaurantId"] = Error.Conflict(
            "Restaurants.HasReservations",
            "This restaurant cannot be deleted while it still has reservations."),

        ["FK_Reservations_Tables_TableId_RestaurantId_TableCapacity"] = Error.Conflict(
            "Tables.HasReservations",
            "This table cannot be deleted while it still has reservations."),

        ["FK_Orders_Reservations_ReservationId_RestaurantId"] = Error.Conflict(
            "Reservations.HasOrders",
            "This reservation cannot be deleted while orders are recorded against it."),

        ["FK_Orders_Employees_EmployeeId_RestaurantId"] = Error.Conflict(
            "Employees.HasOrders",
            "This employee cannot be deleted while orders they took are still recorded."),

        ["FK_OrderItems_MenuItems_ItemId_RestaurantId"] = Error.Conflict(
            "MenuItems.HasOrderItems",
            "This menu item cannot be deleted while it appears on existing orders."),

        ["FK_Employees_Restaurants_RestaurantId"] = Error.Conflict(
            "Restaurants.HasEmployees",
            "This restaurant cannot be deleted while employees are assigned to it."),

        ["FK_MenuItems_Restaurants_RestaurantId"] = Error.Conflict(
            "Restaurants.HasMenuItems",
            "This restaurant cannot be deleted while menu items belong to it."),

        ["FK_Tables_Restaurants_RestaurantId"] = Error.Conflict(
            "Restaurants.HasTables",
            "This restaurant cannot be deleted while tables belong to it."),
    };

    private static Error Lookup(Dictionary<string, Error> catalogue, string? constraint, Error fallback) =>
        constraint is not null && catalogue.TryGetValue(constraint, out var known) ? known : fallback;

    private static bool HasPrefix(string constraint, string prefix) =>
        constraint.StartsWith(prefix, StringComparison.OrdinalIgnoreCase);
}
