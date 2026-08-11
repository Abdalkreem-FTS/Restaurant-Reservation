namespace RestaurantReservation.API.Contracts.Common;

public static class CommonMappers
{
    public static PageRequest ToPageRequest(this PageParameters parameters) => new()
    {
        Number = parameters.Page ?? 1,
        Size = parameters.PageSize ?? PageRequest.DefaultSize
    };

    public static PagedResponse<TResponse> ToPagedResponse<TSource, TResponse>(
        this PagedResult<TSource> page,
        Func<TSource, TResponse> toResponse) =>
        new(
            [.. page.Items.Select(toResponse)],
            page.PageNumber,
            page.PageSize,
            page.TotalCount,
            page.TotalPages,
            page.HasPrevious,
            page.HasNext);
}
