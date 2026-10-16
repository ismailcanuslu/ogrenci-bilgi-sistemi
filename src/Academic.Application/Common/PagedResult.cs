namespace Academic.Application.Common;

public sealed record PagedResult<T>(
    IReadOnlyCollection<T> Items,
    int Page,
    int PageSize,
    int TotalCount)
{
    public int TotalPages => PageSize == 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);
}

public sealed record PagingRequest(int Page = 1, int PageSize = 20)
{
    public const int MaxPageSize = 100;
}

public enum SortDirection
{
    Asc = 0,
    Desc = 1
}
