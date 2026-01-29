namespace HotelPlatform.Application.Common.Pagination;

public class PagedResult<T> where T : class
{
    public IReadOnlyList<T> Items { get; init; } = [];
    public int TotalItems { get; init; }
    public int Page { get; init; }
    public int PerPage { get; init; }

    public int TotalPages => (int)Math.Ceiling(TotalItems / (double)PerPage);
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;

    public static PagedResult<T> Create(
        IReadOnlyList<T> items,
        int totalItems,
        int page,
        int perPage)
    {
        return new PagedResult<T>
        {
            Items = items,
            TotalItems = totalItems,
            Page = page,
            PerPage = perPage
        };
    }
    
}