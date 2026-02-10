// Models/Responses/Hotels/HotelSummaryResponse.cs

namespace HotelManagement.BlazorServer.models.Response.Hotels;


public sealed record HotelSummaryResponse(
    Guid Id,
    string Name,
    string? MainPictureUrl,
    string? City,
    string? Country,
    decimal? AverageRating,
    int TotalReviews,
    int AmenityCount);