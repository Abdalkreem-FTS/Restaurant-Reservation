namespace RestaurantReservation.Db.Pagination;

/// <summary>
/// A requested slice of a result set. Both values are clamped on assignment, so a caller cannot defeat pagination by asking for one enormous page.
/// </summary>
public sealed record PageRequest
{
    public const int DefaultSize = 20;
    public const int MaxSize = 100;

    private readonly int _number = 1;
    private readonly int _size = DefaultSize;

    public int Number
    {
        get => _number;
        init => _number = value < 1 ? 1 : value;
    }

    public int Size
    {
        get => _size;
        init => _size = value < 1 ? DefaultSize : Math.Min(value, MaxSize);
    }

    public int Skip => (Number - 1) * Size;
}
