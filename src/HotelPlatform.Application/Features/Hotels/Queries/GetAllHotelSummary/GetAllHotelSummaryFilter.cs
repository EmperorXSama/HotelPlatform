using HotelPlatform.Application.Common.Pagination;

namespace HotelPlatform.Application.Features.Hotels.Queries.GetAllHotelSummary;


public class GetAllHotelSummaryFilter : PagedRequest
{
    public string? Name { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public decimal? MinRating { get; set; }
    public decimal? MaxRating { get; set; }
    
    public static readonly string[] AllowedOrderings = {"name", "city","country"};
}