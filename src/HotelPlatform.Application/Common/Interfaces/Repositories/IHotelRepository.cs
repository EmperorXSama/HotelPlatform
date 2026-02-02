using HotelPlatform.Domain.Hotels;

namespace HotelPlatform.Application.Common.Interfaces.Repositories;

public interface IHotelRepository
{
    Task<Hotel?> GetByIdAsync(HotelId id, CancellationToken cancellationToken = default);
    Task<Hotel?> GetByIdWithRoomsAsync(HotelId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Hotel>> GetByOwnerIdAsync(UserId ownerId, CancellationToken cancellationToken = default);
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