namespace RestaurantReservation.Db.Entities;

public class Table
{
    public int TableId { get; set; }

    public int RestaurantId { get; set; }

    public int Capacity { get; set; }

    // Navigations
    public Restaurant Restaurant { get; set; } = null!;

    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}