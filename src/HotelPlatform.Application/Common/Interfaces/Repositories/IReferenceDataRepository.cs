using HotelPlatform.Domain.ReferenceData;

namespace HotelPlatform.Application.Common.Interfaces.Repositories;

public interface IReferenceDataRepository
{
    // Hotel Amenities
    Task<HotelAmenityDefinition?> GetHotelAmenityByIdAsync(
        HotelAmenityDefinitionId id, 
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<HotelAmenityDefinition>> GetHotelAmenitiesByIdsAsync(
        IEnumerable<HotelAmenityDefinitionId> ids,
        CancellationToken cancellationToken = default);
    
    Task<HotelAmenityDefinition?> GetHotelAmenityByCodeAsync(
        string code, 
        CancellationToken cancellationToken = default);
    
    Task<IReadOnlyList<HotelAmenityDefinition>> GetActiveHotelAmenitiesAsync(
        CancellationToken cancellationToken = default);
    
    Task AddHotelAmenityAsync(
        HotelAmenityDefinition amenity, 
        CancellationToken cancellationToken = default);
    
    void UpdateHotelAmenity(HotelAmenityDefinition amenity);

    // Room Amenities
    Task<RoomAmenityDefinition?> GetRoomAmenityByIdAsync(
        RoomAmenityDefinitionId id, 
        CancellationToken cancellationToken = default);
    
    Task<RoomAmenityDefinition?> GetRoomAmenityByCodeAsync(
        string code, 
        CancellationToken cancellationToken = default);
    
    Task<IReadOnlyList<RoomAmenityDefinition>> GetActiveRoomAmenitiesAsync(
        CancellationToken cancellationToken = default);
    
    Task AddRoomAmenityAsync(
        RoomAmenityDefinition amenity, 
        CancellationToken cancellationToken = default);
    
    void UpdateRoomAmenity(RoomAmenityDefinition amenity);

    // Room Types
    Task<RoomTypeDefinition?> GetRoomTypeByIdAsync(
        RoomTypeDefinitionId id, 
        CancellationToken cancellationToken = default);
    
    Task<RoomTypeDefinition?> GetRoomTypeByCodeAsync(
        string code, 
        CancellationToken cancellationToken = default);
    
    Task<IReadOnlyList<RoomTypeDefinition>> GetActiveRoomTypesAsync(
        CancellationToken cancellationToken = default);
    
    Task AddRoomTypeAsync(
        RoomTypeDefinition roomType, 
        CancellationToken cancellationToken = default);
    
    void UpdateRoomType(RoomTypeDefinition roomType);
}