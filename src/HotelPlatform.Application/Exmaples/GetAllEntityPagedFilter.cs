using HotelPlatform.Application.Common.Pagination;

namespace HotelPlatform.Application.Exmaples;

public class GetAllEntityPagedFilter : PagedRequest
{
    public string? Name { get; set; }
    public string? Slug { get; set; }
    
    public static readonly string[] AllowedOrderings = {"name", "usageCount","slug"};
}