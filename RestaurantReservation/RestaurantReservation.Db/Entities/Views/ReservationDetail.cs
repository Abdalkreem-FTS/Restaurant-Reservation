namespace RestaurantReservation.Db.Entities.Views;

public class ReservationDetail
{
    public int ReservationId { get; set; }

    public DateTime ReservationDate { get; set; }

    public int PartySize { get; set; }

    // Customer
    public int CustomerId { get; set; }

    public string CustomerFirstName { get; set; } = null!;

    public string CustomerLastName { get; set; } = null!;

    public string CustomerEmail { get; set; } = null!;

    public string CustomerPhoneNumber { get; set; } = null!;

    // Restaurant
    public int RestaurantId { get; set; }

    public string RestaurantName { get; set; } = null!;

    public string RestaurantAddress { get; set; } = null!;

    public string RestaurantPhoneNumber { get; set; } = null!;
}