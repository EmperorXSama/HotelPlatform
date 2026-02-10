namespace HotelPlatform.Application.Features.Hotels.Queries.GetAllHotelSummary;

public sealed record HotelSummaryResponse(
    Guid Id,
    string Name,
    string? City,
    string? Country,
    decimal? AverageRating,
    int ReviewCount,
    string? MainImageUrl,
    int AmenityCount);