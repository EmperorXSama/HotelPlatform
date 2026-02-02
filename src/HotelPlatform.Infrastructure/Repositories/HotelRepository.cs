// Infrastructure/Repositories/HotelRepository.cs
using HotelPlatform.Application.Common.Interfaces.Repositories;
using HotelPlatform.Domain.Common.StronglyTypedIds;
using HotelPlatform.Domain.Enums;
using HotelPlatform.Domain.Hotels;
using HotelPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HotelPlatform.Infrastructure.Repositories;

public class HotelRepository : IHotelRepository
{
    private readonly ApplicationDbContext _dbContext;

    public HotelRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Hotel?> GetByIdAsync(HotelId id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Hotels
            .FirstOrDefaultAsync(h => h.Id == id, cancellationToken);
    }

    public async Task<Hotel?> GetByIdWithRoomsAsync(HotelId id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Hotels
            .Include(h => h.Rooms)
            .FirstOrDefaultAsync(h => h.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Hotel>> GetByOwnerIdAsync(UserId ownerId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Hotels
            .Where(h => h.OwnerId == ownerId)
            .OrderByDescending(h => h.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(HotelId id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Hotels
            .AnyAsync(h => h.Id == id, cancellationToken);
    }

    public async Task AddAsync(Hotel hotel, CancellationToken cancellationToken = default)
    {
        await _dbContext.Hotels.AddAsync(hotel, cancellationToken);
    }

    public void Update(Hotel hotel)
    {
        _dbContext.Hotels.Update(hotel);
    }

    public void Delete(Hotel hotel)
    {
        _dbContext.Hotels.Remove(hotel);
    }

    public async Task<IReadOnlyList<Hotel>> GetPublishedHotelsAsync(
        int skip, 
        int take, 
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Hotels
            .Where(h => h.Status == HotelStatus.Published)
            .OrderByDescending(h => h.AggregatedRating.AverageScore)
            .ThenByDescending(h => h.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountByStatusAsync(HotelStatus status, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Hotels
            .CountAsync(h => h.Status == status, cancellationToken);
    }
}