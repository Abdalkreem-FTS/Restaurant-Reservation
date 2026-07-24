namespace RestaurantReservation.Db.Entities;

public class Reservation
{
    public int ReservationId { get; set; }

    public int CustomerId { get; set; }

    public int RestaurantId { get; set; }

    public int TableId { get; set; }

    public DateTime ReservationDate { get; set; }

    public int PartySize { get; set; }

    // Navigations
    public Customer Customer { get; set; } = null!;

    public Restaurant Restaurant { get; set; } = null!;

    public Table Table { get; set; } = null!;

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}