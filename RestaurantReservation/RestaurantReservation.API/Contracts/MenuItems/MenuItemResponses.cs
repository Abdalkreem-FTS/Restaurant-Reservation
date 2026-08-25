namespace RestaurantReservation.API.Contracts.MenuItems;

public sealed record MenuItemResponse(
    int ItemId,
    int RestaurantId,
    string Name,
    string? Description,
    decimal Price);
