using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantReservation.Db.Entities.Views;

namespace RestaurantReservation.Db.Configurations.Views;

public class EmployeeRestaurantDetailConfiguration : IEntityTypeConfiguration<EmployeeRestaurantDetail>
{
    public void Configure(EntityTypeBuilder<EmployeeRestaurantDetail> builder)
    {
        builder.HasNoKey().ToView("vw_EmployeeDetails");
    }
}