using RestaurantReservation.Db.Enums;

namespace RestaurantReservation.Db.Entities;

public class Employee : IVersionedEntity
{
    public int EmployeeId { get; set; }

    public int RestaurantId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public EmployeePosition Position { get; set; }

    public int? UserId { get; set; }

    public byte[] RowVersion { get; set; } = [];

    // Navigations
    public User? User { get; set; }

    public Restaurant Restaurant { get; set; } = null!;

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
