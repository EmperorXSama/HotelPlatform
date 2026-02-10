// Models/Requests/Hotels/GetHotelsRequest.cs

using HotelManagement.BlazorServer.Models.Pagination;

namespace HotelManagement.BlazorServer.models.Request.Hotels;

/// <summary>
/// Request for fetching hotels with filtering and pagination
/// </summary>
public sealed class GetHotelsRequest : PagedRequest
{
    public string? Name { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public decimal? MinRating { get; set; }
    public decimal? MaxRating { get; set; }

    public override Dictionary<string, string> ToQueryParameters()
    {
        var parameters = base.ToQueryParameters();

        if (!string.IsNullOrWhiteSpace(Name))
            parameters["name"] = Name;

        if (!string.IsNullOrWhiteSpace(City))
            parameters["city"] = City;

        if (!string.IsNullOrWhiteSpace(Country))
            parameters["country"] = Country;

        if (MinRating.HasValue)
            parameters["minRating"] = MinRating.Value.ToString();

        if (MaxRating.HasValue)
            parameters["maxRating"] = MaxRating.Value.ToString();

        return parameters;
    }
}