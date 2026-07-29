using RestaurantReservation.Db.Entities;

namespace RestaurantReservation.Db.Configurations;

public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.HasKey(r => r.ReservationId);
        
        builder.HasAlternateKey(r => new { r.ReservationId, r.RestaurantId });

        builder.ToTable(t =>
        {
            t.HasCheckConstraint("CK_Reservations_PartySizeIsPositive", "[PartySize] > 0");

            t.HasCheckConstraint("CK_Reservations_PartySizeWithinTableCapacity", "[PartySize] <= [TableCapacity]");

            t.HasCheckConstraint(
                "CK_Reservations_ReservationDateOnTheHour",
                "DATEPART(MINUTE, [ReservationDate]) = 0 AND DATEPART(SECOND, [ReservationDate]) = 0 AND DATEPART(NANOSECOND, [ReservationDate]) = 0");
        });

        builder.HasIndex(r => new { r.TableId, r.ReservationDate })
            .IsUnique();

        builder.Property(r => r.ReservationDate)
            .IsRequired();

        builder.Property(r => r.PartySize)
            .IsRequired();
        
        builder.HasOne(r => r.Customer)
            .WithMany(c => c.Reservations)
            .HasForeignKey(r => r.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Restaurant)
            .WithMany(res => res.Reservations)
            .HasForeignKey(r => r.RestaurantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Table)
            .WithMany(t => t.Reservations)
            .HasForeignKey(r => new { r.TableId, r.RestaurantId, r.TableCapacity })
            .HasPrincipalKey(t => new { t.TableId, t.RestaurantId, t.Capacity })
            .OnDelete(DeleteBehavior.Restrict);
    }
}
