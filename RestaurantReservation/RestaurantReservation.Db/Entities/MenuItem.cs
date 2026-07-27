namespace RestaurantReservation.Db.Entities;

public class MenuItem : IVersionedEntity
{
    public int ItemId { get; set; }

    public int RestaurantId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public byte[] RowVersion { get; set; } = [];

    // Navigations
    public Restaurant Restaurant { get; set; } = null!;

    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
