// Infrastructure/Repositories/StoredFileRepository.cs
using HotelPlatform.Application.Common.Interfaces.Repositories;
using HotelPlatform.Application.Common.Interfaces.Storage;
using HotelPlatform.Domain.Common.StronglyTypedIds;
using HotelPlatform.Domain.Files;
using HotelPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HotelPlatform.Infrastructure.Repositories;

public class StoredFileRepository : IStoredFileRepository
{
    private readonly ApplicationDbContext _dbContext;

    public StoredFileRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<StoredFile?> GetByIdAsync(StoredFileId id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.StoredFiles
            .FirstOrDefaultAsync(f => f.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<StoredFile>> GetByOwnerIdAsync(UserId ownerId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.StoredFiles
            .Where(f => f.OwnerId == ownerId)
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(StoredFileId id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.StoredFiles
            .AnyAsync(f => f.Id == id, cancellationToken);
    }

    public async Task AddAsync(StoredFile file, CancellationToken cancellationToken = default)
    {
        await _dbContext.StoredFiles.AddAsync(file, cancellationToken);
    }

    public void Delete(StoredFile file)
    {
        _dbContext.StoredFiles.Remove(file);
    }

    public async Task<bool> IsInUseAsync(StoredFileId id, CancellationToken cancellationToken = default)
    {
        // Check if file is used in any hotel pictures
        var usedInHotel = await _dbContext.Hotels
            .AnyAsync(h => h.Pictures.Any(p => p.StoredFileId == id), cancellationToken);

        if (usedInHotel)
            return true;

        // Check if file is used in any room pictures
        var usedInRoom = await _dbContext.Rooms
            .AnyAsync(r => r.Pictures.Any(p => p.StoredFileId == id), cancellationToken);

        return usedInRoom;
    }
}