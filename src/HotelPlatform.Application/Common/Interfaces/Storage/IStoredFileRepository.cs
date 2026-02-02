using HotelPlatform.Domain.Files;

namespace HotelPlatform.Application.Common.Interfaces.Storage;

public interface IStoredFileRepository
{
    Task<StoredFile?> GetByIdAsync(StoredFileId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StoredFile>> GetByOwnerIdAsync(UserId ownerId, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(StoredFileId id, CancellationToken cancellationToken = default);
    Task AddAsync(StoredFile file, CancellationToken cancellationToken = default);
    void Delete(StoredFile file);
    
    // Check if file is used by any hotel or room
    Task<bool> IsInUseAsync(StoredFileId id, CancellationToken cancellationToken = default);
}