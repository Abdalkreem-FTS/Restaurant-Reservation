using RestaurantReservation.API.Contracts.Common;

namespace RestaurantReservation.API.Contracts.MenuItems;

public static class MenuItemMappers
{
    public static MenuItemResponse ToResponse(this MenuItem menuItem) =>
        new(
            menuItem.ItemId,
            menuItem.RestaurantId,
            menuItem.Name,
            menuItem.Description,
            menuItem.Price);

    public static PagedResponse<MenuItemResponse> ToResponse(this PagedResult<MenuItem> page) =>
        page.ToPagedResponse(ToResponse);
}
