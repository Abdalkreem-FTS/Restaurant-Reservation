namespace RestaurantReservation.Db.Entities;

public class OrderItem
{
    public int OrderItemId { get; set; }

    public int OrderId { get; set; }

    public int ItemId { get; set; }

    public int Quantity { get; set; }

    // Navigations
    public Order Order { get; set; } = null!;

    public MenuItem MenuItem { get; set; } = null!;
}