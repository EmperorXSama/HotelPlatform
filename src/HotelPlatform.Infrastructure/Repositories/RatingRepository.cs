// Infrastructure/Repositories/RatingRepository.cs
using HotelPlatform.Application.Common.Interfaces.Repositories;
using HotelPlatform.Domain.Common.StronglyTypedIds;
using HotelPlatform.Domain.Ratings;
using HotelPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HotelPlatform.Infrastructure.Repositories;

public class RatingRepository : IRatingRepository
{
    private readonly ApplicationDbContext _dbContext;

    public RatingRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Rating?> GetByIdAsync(RatingId id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Ratings
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<Rating?> GetByHotelAndUserAsync(HotelId hotelId, UserId userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Ratings
            .FirstOrDefaultAsync(r => r.HotelId == hotelId && r.UserId == userId, cancellationToken);
    }

    public async Task<IReadOnlyList<Rating>> GetByHotelIdAsync(HotelId hotelId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Ratings
            .Where(r => r.HotelId == hotelId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(HotelId hotelId, UserId userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Ratings
            .AnyAsync(r => r.HotelId == hotelId && r.UserId == userId, cancellationToken);
    }

    public async Task AddAsync(Rating rating, CancellationToken cancellationToken = default)
    {
        await _dbContext.Ratings.AddAsync(rating, cancellationToken);
    }

    public void Update(Rating rating)
    {
        _dbContext.Ratings.Update(rating);
    }

    public void Delete(Rating rating)
    {
        _dbContext.Ratings.Remove(rating);
    }

    public async Task<(decimal AverageScore, int TotalCount)> GetAggregatedRatingAsync(
        HotelId hotelId, 
        CancellationToken cancellationToken = default)
    {
        var result = await _dbContext.Ratings
            .Where(r => r.HotelId == hotelId)
            .GroupBy(_ => 1)
            .Select(g => new
            {
                Average = g.Average(r => (decimal)r.Score),
                Count = g.Count()
            })
            .FirstOrDefaultAsync(cancellationToken);

        return result is null 
            ? (0, 0) 
            : (Math.Round(result.Average, 2), result.Count);
    }
}