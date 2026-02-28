namespace SwiggyClone.Shared;

/// <summary>
/// Cursor-based (keyset) paginated result. Preferred for all large datasets.
/// Provides O(1) page retrieval regardless of dataset size.
/// </summary>
public sealed class CursorPagedResult<T>
{
    public IReadOnlyList<T> Items { get; }
    public string? NextCursor { get; }
    public string? PreviousCursor { get; }
    public bool HasMore { get; }
    public int PageSize { get; }

    public CursorPagedResult(IReadOnlyList<T> items, string? nextCursor, string? previousCursor, bool hasMore, int pageSize)
    {
        Items = items;
        NextCursor = nextCursor;
        PreviousCursor = previousCursor;
        HasMore = hasMore;
        PageSize = pageSize;
    }
}
