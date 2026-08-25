namespace RestaurantReservation.Db.Entities;

public class Customer : IVersionedEntity
{
    public int CustomerId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public int? UserId { get; set; }

    public byte[] RowVersion { get; set; } = [];

    // Navigations
    public User? User { get; set; }

    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
