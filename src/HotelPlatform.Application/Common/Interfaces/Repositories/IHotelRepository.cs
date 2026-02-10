using HotelPlatform.Application.Common.Pagination;
using HotelPlatform.Application.Features.Hotels.Common;
using HotelPlatform.Application.Features.Hotels.Queries.GetAllHotelSummary;
using HotelPlatform.Domain.Hotels;

namespace HotelPlatform.Application.Common.Interfaces.Repositories;

public interface IHotelRepository
{
    Task<ErrorOr<PagedResult<HotelSummaryResponse>>> GetPagedSummaryHotelAsync(GetAllHotelSummaryFilter filter, CancellationToken cancellationToken = default);
    
    Task<Hotel?> GetByIdAsync(HotelId id, CancellationToken cancellationToken = default);
    Task<Hotel?> GetByIdWithAmenitiesAsync(
        HotelId id,
        CancellationToken cancellationToken = default);
    Task<Hotel?> GetByIdWithDetailsAsync(HotelId id, CancellationToken cancellationToken = default);
    Task<Hotel?> GetByIdWithRoomsAsync(HotelId id, CancellationToken cancellationToken = default);
    Task<List<Hotel>> GetByOwnerIdAsync(UserId ownerId, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(HotelId id, CancellationToken cancellationToken = default);
    Task AddAsync(Hotel hotel, CancellationToken cancellationToken = default);
    void Update(Hotel hotel);
    void Delete(Hotel hotel);
    
    // Query methods
    Task<IReadOnlyList<Hotel>> GetPublishedHotelsAsync(
        int skip, 
        int take, 
        CancellationToken cancellationToken = default);
    
    Task<int> CountByStatusAsync(HotelStatus status, CancellationToken cancellationToken = default);
}