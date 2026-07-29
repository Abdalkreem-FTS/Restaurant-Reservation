namespace RestaurantReservation.Db.Pagination;

public static class QueryablePaginationExtensions
{
    public static async Task<PagedResult<T>> GetPageAsync<T>(this IQueryable<T> query, PageRequest page, CancellationToken cancellationToken = default)
    {
        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip(page.Skip)
            .Take(page.Size)
            .ToListAsync(cancellationToken);

        return new PagedResult<T>(items, totalCount, page.Number, page.Size);
    }
}
