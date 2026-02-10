// Infrastructure/Repositories/HotelRepository.cs

using ErrorOr;
using HotelPlatform.Application.Common.Interfaces.Repositories;
using HotelPlatform.Application.Common.Interfaces.Storage;
using HotelPlatform.Application.Common.Pagination;
using HotelPlatform.Application.Features.Hotels.Common;
using HotelPlatform.Application.Features.Hotels.Queries.GetAllHotelSummary;
using HotelPlatform.Domain.Common.StronglyTypedIds;
using HotelPlatform.Domain.Enums;
using HotelPlatform.Domain.Hotels;
using HotelPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HotelPlatform.Infrastructure.Repositories;

public class HotelRepository : IHotelRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IFileUrlResolver _fileUrlResolver;
    public HotelRepository(ApplicationDbContext dbContext, IFileUrlResolver fileUrlResolver)
    {
        _dbContext = dbContext;
        _fileUrlResolver = fileUrlResolver;
    }

    public async Task<ErrorOr<PagedResult<HotelSummaryResponse>>> GetPagedSummaryHotelAsync(GetAllHotelSummaryFilter filter, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Hotels.AsNoTracking();

        query = ApplyFilter(query, filter);
        var totalCount = await query.CountAsync(cancellationToken);

        query = ApplyOrdering(query, filter);
        var items = await query
            .Skip(filter.Skip)
            .Take(filter.PageSize)
            .Select(h => new HotelSummaryResponse(
                h.Id, 
                h.Name, 
                h.Address!.City,
                h.Address.Country,
                h.AggregatedRating.AverageScore,
                h.AggregatedRating.TotalCounts,
                _fileUrlResolver.GetAccessUrl(h.Pictures.FirstOrDefault( h => h.IsMain).StoredFileId),
                h.Amenities.Count))
            .ToListAsync(cancellationToken);

        return PagedResult<HotelSummaryResponse>.Create(items, totalCount, filter.Page, filter.PageSize);
    }

    public async Task<Hotel?> GetByIdAsync(HotelId id, CancellationToken cancellationToken = default)
    {
        
        return await _dbContext.Hotels
            .FirstOrDefaultAsync(h => h.Id == id, cancellationToken);
    }

 public async Task<Hotel?> GetByIdWithAmenitiesAsync(
    HotelId id,
    CancellationToken cancellationToken = default)
{
    return await _dbContext.Hotels
        .Include(h => h.Amenities)
        .FirstOrDefaultAsync(h => h.Id == id, cancellationToken);
}
    public async Task<Hotel?> GetByIdWithRoomsAsync(HotelId id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Hotels
            .Include(h => h.Rooms)
            .FirstOrDefaultAsync(h => h.Id == id, cancellationToken);
    }

    public async Task<List<Hotel>> GetByOwnerIdAsync(UserId ownerId, CancellationToken cancellationToken = default)
    {

        return await  _dbContext.Hotels
            .AsNoTracking()
            .Include(h => h.Pictures)
            .Include(h => h.Amenities)
            .Include(h => h.Rooms).ToListAsync(cancellationToken);
        
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
    
    public async Task<Hotel?> GetByIdWithDetailsAsync(
        HotelId id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Hotels
            .Include(h => h.Pictures)
            .Include(h => h.Amenities)
            .Include(h => h.Rooms)
            .FirstOrDefaultAsync(h => h.Id == id, cancellationToken);
    }


    private static IQueryable<Hotel> ApplyFilter(IQueryable<Hotel> query,
        GetAllHotelSummaryFilter filter)
    {
        if (!string.IsNullOrWhiteSpace(filter.Name))
        {
            query = query.Where(h => h.Name.ToLower().Contains(filter.Name.ToLower()));
        }
        if (!string.IsNullOrWhiteSpace(filter.City))
        {
            query =  query.Where(h => h.Address.City.ToLower().Contains(filter.City.ToLower()));
        }
        if (filter.MaxRating > 0)
        {
            query = query.Where(h => h.AggregatedRating.AverageScore < filter.MaxRating);
        } 
        if (filter.MinRating > 0)
        {
            query = query.Where(h => h.AggregatedRating.AverageScore < filter.MinRating);
        }
        if (!string.IsNullOrWhiteSpace(filter.Country))
        {
            query = query.Where(h => h.Address.Country.ToLower().Contains(filter.Country.ToLower()));
        }
        return query;
    }
    private static IQueryable<Hotel> ApplyOrdering(IQueryable<Hotel> query,
        GetAllHotelSummaryFilter filter)
    {
        var orderby = filter.OrderBy?.ToLower();

        if (string.IsNullOrWhiteSpace(orderby) || !GetAllHotelSummaryFilter.AllowedOrderings.Contains(orderby)) 
        {
            return filter.IsDescending
                ? query.OrderByDescending(h => h.AggregatedRating.AverageScore)
                : query.OrderBy(h =>h.AggregatedRating.AverageScore);
        }
        
        return orderby switch
        {
            "name" => filter.IsDescending
                ? query.OrderByDescending(h => h.Name)
                : query.OrderBy(h => h.Name),
            "city" => filter.IsDescending
            ? query.OrderByDescending(h => h.Address.City)
            : query.OrderBy(h => h.Address.City),
            "country" => filter.IsDescending
                ? query.OrderByDescending(h => h.Address.Country)
                : query.OrderBy(h => h.Address.Country),
            _ => query
        };
    }
}