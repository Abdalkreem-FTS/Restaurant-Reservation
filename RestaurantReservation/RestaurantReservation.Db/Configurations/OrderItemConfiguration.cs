using RestaurantReservation.Db.Entities;

namespace RestaurantReservation.Db.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.HasKey(oi => oi.OrderItemId);

        builder.ToTable(t =>
        {
            t.HasCheckConstraint("CK_OrderItems_UnitPriceIsNotNegative", "[UnitPrice] >= 0");

            t.HasCheckConstraint("CK_OrderItems_QuantityIsPositive", "[Quantity] > 0");
        });

        builder.Property(oi => oi.Quantity)
            .IsRequired();

        builder.Property(oi => oi.UnitPrice)
            .HasPrecision(18, 2);

        builder.HasOne(oi => oi.Order)
            .WithMany(o => o.OrderItems)
            .HasForeignKey(oi => new { oi.OrderId, oi.RestaurantId })
            .HasPrincipalKey(o => new { o.OrderId, o.RestaurantId })
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(oi => oi.MenuItem)
            .WithMany(m => m.OrderItems)
            .HasForeignKey(oi => new { oi.ItemId, oi.RestaurantId })
            .HasPrincipalKey(m => new { m.ItemId, m.RestaurantId })
            .OnDelete(DeleteBehavior.Restrict);
    }
}
