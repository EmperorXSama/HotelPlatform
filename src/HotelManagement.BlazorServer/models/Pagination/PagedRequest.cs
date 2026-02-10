// Models/Pagination/PagedRequest.cs

namespace HotelManagement.BlazorServer.Models.Pagination;

/// <summary>
/// Base class for paginated requests - mirrors the API's PagedRequest
/// </summary>
public class PagedRequest
{
    private const int DefaultPageSize = 10;
    private const int MaxPageSize = 100;

    private int _page = 1;
    private int _pageSize = DefaultPageSize;

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

    public string? OrderBy { get; set; }
    public string SortOrder { get; set; } = "asc";

    /// <summary>
    /// Converts to query parameters dictionary for HTTP requests
    /// </summary>
    public virtual Dictionary<string, string> ToQueryParameters()
    {
        var parameters = new Dictionary<string, string>
        {
            ["page"] = Page.ToString(),
            ["pageSize"] = PageSize.ToString(),
            ["sortOrder"] = SortOrder
        };

        if (!string.IsNullOrWhiteSpace(OrderBy))
        {
            parameters["orderBy"] = OrderBy;
        }

        return parameters;
    }
}