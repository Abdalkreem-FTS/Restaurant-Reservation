namespace RestaurantReservation.Db.Entities;

public class Customer
{
    public int CustomerId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    // Navigation
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}