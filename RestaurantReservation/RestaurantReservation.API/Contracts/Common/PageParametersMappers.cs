namespace RestaurantReservation.API.Contracts.Common;

public static class PageParametersMappers
{
    public static PageRequest ToPageRequest(this PageParameters parameters) => new()
    {
        Number = parameters.Page ?? 1,
        Size = parameters.PageSize ?? PageRequest.DefaultSize
    };
}
