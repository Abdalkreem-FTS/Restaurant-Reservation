namespace RestaurantReservation.Db.Entities;

public class Order : IVersionedEntity
{
    public int OrderId { get; set; }

    public int ReservationId { get; set; }

    public int EmployeeId { get; set; }

    /// <summary>
    /// Denormalized from the reservation so the composite foreign keys to <see cref="Reservation" />
    /// and <see cref="Employee" /> can force both to belong to the same restaurant.
    /// </summary>
    public int RestaurantId { get; set; }

    public DateTimeOffset OrderDate { get; set; }

    /// <summary>
    /// Derived from the order's items rather than set directly, so it can never disagree with the
    /// prices actually charged. Assignable only from inside the aggregate; call
    /// <see cref="RecalculateTotal" /> after changing <see cref="OrderItems" />.
    /// </summary>
    public decimal TotalAmount { get; internal set; }

    public byte[] RowVersion { get; set; } = [];

    // Navigations
    public Reservation Reservation { get; set; } = null!;

    public Employee Employee { get; set; } = null!;

    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public void RecalculateTotal() => TotalAmount = OrderItems.Sum(item => item.UnitPrice * item.Quantity);
}
