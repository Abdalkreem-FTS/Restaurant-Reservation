namespace RestaurantReservation.Db.Entities;

public class Reservation : IVersionedEntity
{
    public int ReservationId { get; set; }

    public int CustomerId { get; set; }

    public int RestaurantId { get; set; }

    public int TableId { get; set; }

    /// <summary>
    /// Start of the booking, always falling exactly on the hour. A reservation occupies its table
    /// for one hour, so slots can never partially overlap and a unique index over the table and this
    /// column is enough to prevent double booking.
    /// </summary>
    public DateTimeOffset ReservationDate { get; set; }

    public int PartySize { get; set; }

    /// <summary>
    /// Mirrors <see cref="Entities.Table.Capacity" /> through the composite foreign key to
    /// <see cref="Table" />, so the party size can be bounded by a single-row check constraint
    /// instead of a cross-table lookup.
    /// </summary>
    public int TableCapacity { get; set; }

    public byte[] RowVersion { get; set; } = [];

    // Navigations
    public Customer Customer { get; set; } = null!;

    public Restaurant Restaurant { get; set; } = null!;

    public Table Table { get; set; } = null!;

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
