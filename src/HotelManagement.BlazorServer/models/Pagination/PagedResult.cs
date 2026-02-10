// Models/Pagination/PagedResult.cs

namespace HotelManagement.BlazorServer.Models.Pagination;

/// <summary>
/// Paginated response wrapper - mirrors the API's PagedResult
/// </summary>
public sealed class PagedResult<T> where T : class
{
    public IReadOnlyList<T> Items { get; init; } = [];
    public int TotalItems { get; init; }
    public int Page { get; init; }
    public int PerPage { get; init; }

    public int TotalPages => PerPage > 0 
        ? (int)Math.Ceiling(TotalItems / (double)PerPage) 
        : 0;
    
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;

    /// <summary>
    /// Creates an empty result
    /// </summary>
    public static PagedResult<T> Empty() => new()
    {
        Items = [],
        TotalItems = 0,
        Page = 1,
        PerPage = 10
    };
}