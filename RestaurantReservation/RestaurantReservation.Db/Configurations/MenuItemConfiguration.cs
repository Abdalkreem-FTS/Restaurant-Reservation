using RestaurantReservation.Db.Entities;

namespace RestaurantReservation.Db.Configurations;

public class MenuItemConfiguration : IEntityTypeConfiguration<MenuItem>
{
    public void Configure(EntityTypeBuilder<MenuItem> builder)
    {
        builder.HasKey(m => m.ItemId);
        
        builder.HasAlternateKey(m => new { m.ItemId, m.RestaurantId });

        builder.ToTable(t => t.HasCheckConstraint("CK_MenuItems_PriceIsNotNegative", "[Price] >= 0"));

        builder.Property(m => m.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(m => m.Description)
            .HasMaxLength(500);

        builder.Property(m => m.Price)
            .HasPrecision(18, 2);

        builder.HasOne(m => m.Restaurant)
            .WithMany(r => r.MenuItems)
            .HasForeignKey(m => m.RestaurantId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
