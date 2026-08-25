using Riok.Mapperly.Abstractions;

namespace RestaurantReservation.API.Contracts.MenuItems;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class MenuItemMappers
{
    private static partial MenuItemResponse ToResponse(this MenuItem menuItem);

    public static partial PagedResponse<MenuItemResponse> ToResponse(this PagedResult<MenuItem> page);
}
