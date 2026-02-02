// Infrastructure/Repositories/ReferenceDataRepository.cs
using HotelPlatform.Application.Common.Interfaces.Repositories;
using HotelPlatform.Domain.Common.StronglyTypedIds;
using HotelPlatform.Domain.ReferenceData;
using HotelPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HotelPlatform.Infrastructure.Repositories;

public class ReferenceDataRepository : IReferenceDataRepository
{
    private readonly ApplicationDbContext _dbContext;

    public ReferenceDataRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    #region Hotel Amenities

    public async Task<HotelAmenityDefinition?> GetHotelAmenityByIdAsync(
        HotelAmenityDefinitionId id, 
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.HotelAmenityDefinitions
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<HotelAmenityDefinition?> GetHotelAmenityByCodeAsync(
        string code, 
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.HotelAmenityDefinitions
            .FirstOrDefaultAsync(a => a.Code == code.ToUpperInvariant(), cancellationToken);
    }

    public async Task<IReadOnlyList<HotelAmenityDefinition>> GetActiveHotelAmenitiesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.HotelAmenityDefinitions
            .Where(a => a.IsActive)
            .OrderBy(a => a.Category)
            .ThenBy(a => a.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task AddHotelAmenityAsync(
        HotelAmenityDefinition amenity, 
        CancellationToken cancellationToken = default)
    {
        await _dbContext.HotelAmenityDefinitions.AddAsync(amenity, cancellationToken);
    }

    public void UpdateHotelAmenity(HotelAmenityDefinition amenity)
    {
        _dbContext.HotelAmenityDefinitions.Update(amenity);
    }

    #endregion

    #region Room Amenities

    public async Task<RoomAmenityDefinition?> GetRoomAmenityByIdAsync(
        RoomAmenityDefinitionId id, 
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.RoomAmenityDefinitions
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<RoomAmenityDefinition?> GetRoomAmenityByCodeAsync(
        string code, 
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.RoomAmenityDefinitions
            .FirstOrDefaultAsync(a => a.Code == code.ToUpperInvariant(), cancellationToken);
    }

    public async Task<IReadOnlyList<RoomAmenityDefinition>> GetActiveRoomAmenitiesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.RoomAmenityDefinitions
            .Where(a => a.IsActive)
            .OrderBy(a => a.Category)
            .ThenBy(a => a.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task AddRoomAmenityAsync(
        RoomAmenityDefinition amenity, 
        CancellationToken cancellationToken = default)
    {
        await _dbContext.RoomAmenityDefinitions.AddAsync(amenity, cancellationToken);
    }

    public void UpdateRoomAmenity(RoomAmenityDefinition amenity)
    {
        _dbContext.RoomAmenityDefinitions.Update(amenity);
    }

    #endregion

    #region Room Types

    public async Task<RoomTypeDefinition?> GetRoomTypeByIdAsync(
        RoomTypeDefinitionId id, 
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.RoomTypeDefinitions
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<RoomTypeDefinition?> GetRoomTypeByCodeAsync(
        string code, 
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.RoomTypeDefinitions
            .FirstOrDefaultAsync(r => r.Code == code.ToUpperInvariant(), cancellationToken);
    }

    public async Task<IReadOnlyList<RoomTypeDefinition>> GetActiveRoomTypesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.RoomTypeDefinitions
            .Where(r => r.IsActive)
            .OrderBy(r => r.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task AddRoomTypeAsync(
        RoomTypeDefinition roomType, 
        CancellationToken cancellationToken = default)
    {
        await _dbContext.RoomTypeDefinitions.AddAsync(roomType, cancellationToken);
    }

    public void UpdateRoomType(RoomTypeDefinition roomType)
    {
        _dbContext.RoomTypeDefinitions.Update(roomType);
    }

    #endregion
}