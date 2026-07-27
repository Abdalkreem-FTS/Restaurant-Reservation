using RestaurantReservation.Db.Entities;

namespace RestaurantReservation.Db.Configurations;

public class TableConfiguration : IEntityTypeConfiguration<Table>
{
    public void Configure(EntityTypeBuilder<Table> builder)
    {
        builder.HasKey(t => t.TableId);
        
        builder.HasAlternateKey(t => new { t.TableId, t.RestaurantId, t.Capacity });

        builder.ToTable(t => t.HasCheckConstraint("CK_Tables_CapacityIsPositive", "[Capacity] > 0"));

        builder.Property(t => t.Capacity)
            .IsRequired();

        builder.HasOne(t => t.Restaurant)
            .WithMany(r => r.Tables)
            .HasForeignKey(t => t.RestaurantId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
