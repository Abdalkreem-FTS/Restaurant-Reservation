namespace RestaurantReservation.Db.Pagination;

public sealed record PagedResult<T>(IReadOnlyList<T> Items, int TotalCount, int PageNumber, int PageSize)
{
    public int TotalPages => PageSize < 1 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);

    public bool HasPrevious => PageNumber > 1;

    public bool HasNext => PageNumber < TotalPages;
}
