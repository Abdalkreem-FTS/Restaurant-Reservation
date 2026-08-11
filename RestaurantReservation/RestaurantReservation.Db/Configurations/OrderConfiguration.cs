using RestaurantReservation.Db.Entities;

namespace RestaurantReservation.Db.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.OrderId);
        
        builder.HasAlternateKey(o => new { o.OrderId, o.RestaurantId });

        builder.Property(o => o.OrderDate)
            .IsRequired();

        builder.Property(o => o.TotalAmount)
            .HasPrecision(18, 2);

        builder.HasOne(o => o.Reservation)
            .WithMany(r => r.Orders)
            .HasForeignKey(o => new { o.ReservationId, o.RestaurantId })
            .HasPrincipalKey(r => new { r.ReservationId, r.RestaurantId })
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.Employee)
            .WithMany(e => e.Orders)
            .HasForeignKey(o => new { o.EmployeeId, o.RestaurantId })
            .HasPrincipalKey(e => new { e.EmployeeId, e.RestaurantId })
            .OnDelete(DeleteBehavior.Restrict);
    }
}
