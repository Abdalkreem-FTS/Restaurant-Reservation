using RestaurantReservation.Db.Entities.Views;

namespace RestaurantReservation.Db.Configurations.Views;

public class ReservationDetailConfiguration : IEntityTypeConfiguration<ReservationDetail>
{
    public void Configure(EntityTypeBuilder<ReservationDetail> builder) => builder.HasNoKey().ToView("vw_ReservationDetails");
}
