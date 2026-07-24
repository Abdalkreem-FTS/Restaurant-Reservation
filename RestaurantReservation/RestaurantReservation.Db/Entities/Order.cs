namespace RestaurantReservation.Db.Entities;

public class Order
{
    public int OrderId { get; set; }

    public int ReservationId { get; set; }

    public int EmployeeId { get; set; }

    public DateTime OrderDate { get; set; }

    public decimal TotalAmount { get; set; }

    // Navigations
    public Reservation Reservation { get; set; } = null!;

    public Employee Employee { get; set; } = null!;

    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}