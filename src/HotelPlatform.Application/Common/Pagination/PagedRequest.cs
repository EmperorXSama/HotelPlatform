namespace HotelPlatform.Application.Common.Pagination;

public class PagedRequest
{
    private const int DefaultPageSize = 10;
    public const int MaxPageSize  = 100;

    private int _page = 1;
    public int _pageSize = DefaultPageSize;

    public int Page
    {
        get => _page;
        set => _page = value > 0 ? value : 1;
    }

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value switch
        {
            < 1 => DefaultPageSize,
            > MaxPageSize => MaxPageSize,
            _ => value
        };
    }
    
    public int Skip => (Page -1 ) * PageSize;
    public bool IsDescending => SortOrder.Equals("desc", StringComparison.OrdinalIgnoreCase);
    public string? OrderBy { get; set; }
    public string SortOrder { get; set; } = "asc";
}