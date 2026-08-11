namespace RestaurantReservation.Db.Entities;

public class User : IVersionedEntity
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public byte[] RowVersion { get; set; } = [];

    // Navigations
    public Customer? Customer { get; set; }

    public Employee? Employee { get; set; }
}
