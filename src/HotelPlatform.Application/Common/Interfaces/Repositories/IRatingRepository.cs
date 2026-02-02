using HotelPlatform.Domain.Ratings;

namespace HotelPlatform.Application.Common.Interfaces.Repositories;

public interface IRatingRepository
{
    Task<Rating?> GetByIdAsync(RatingId id, CancellationToken cancellationToken = default);
    Task<Rating?> GetByHotelAndUserAsync(HotelId hotelId, UserId userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Rating>> GetByHotelIdAsync(HotelId hotelId, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(HotelId hotelId, UserId userId, CancellationToken cancellationToken = default);
    Task AddAsync(Rating rating, CancellationToken cancellationToken = default);
    void Update(Rating rating);
    void Delete(Rating rating);
    
    // Aggregation methods for calculating hotel rating
    Task<(decimal AverageScore, int TotalCount)> GetAggregatedRatingAsync(
        HotelId hotelId, 
        CancellationToken cancellationToken = default);
}