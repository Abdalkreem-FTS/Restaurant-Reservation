namespace RestaurantReservation.Db.Entities;

public class OrderItem : IVersionedEntity
{
    public int OrderItemId { get; set; }

    public int OrderId { get; set; }

    public int ItemId { get; set; }

    /// <summary>
    /// Denormalized from the order so the composite foreign keys to <see cref="Order" /> and
    /// <see cref="MenuItem" /> can force both to belong to the same restaurant.
    /// </summary>
    public int RestaurantId { get; set; }

    public int Quantity { get; set; }

    /// <summary>
    /// Price charged for a single unit when the order was placed, snapshot from
    /// <see cref="MenuItem.Price" />. It deliberately does not follow later menu price changes, so
    /// the historical value of an order stays reconstructable.
    /// </summary>
    public decimal UnitPrice { get; set; }

    public byte[] RowVersion { get; set; } = [];

    // Navigations
    public Order Order { get; set; } = null!;

    public MenuItem MenuItem { get; set; } = null!;
}
