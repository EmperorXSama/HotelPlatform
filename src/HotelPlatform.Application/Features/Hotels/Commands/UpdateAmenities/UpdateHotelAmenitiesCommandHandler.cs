using HotelPlatform.Application.Common.Interfaces;
using HotelPlatform.Application.Common.Interfaces.Repositories;
using HotelPlatform.Domain.Common.ValueObjects;
using HotelPlatform.Domain.Hotels.Entities;
using Microsoft.Extensions.Logging;

namespace HotelPlatform.Application.Features.Hotels.Commands.UpdateAmenities;

public class UpdateHotelAmenitiesCommandHandler : IRequestHandler<UpdateHotelAmenitiesCommand,ErrorOr<List<Guid>>>
{
    private readonly IReferenceDataRepository _referenceRepository;
    private readonly IHotelRepository _hotelRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<UpdateHotelAmenitiesCommandHandler> _logger;
    public UpdateHotelAmenitiesCommandHandler(IReferenceDataRepository referenceRepository, IUserRepository userRepository, ICurrentUser currentUser, IHotelRepository hotelRepository, ILogger<UpdateHotelAmenitiesCommandHandler> logger)
    {
        _referenceRepository = referenceRepository;
        _userRepository = userRepository;
        _currentUser = currentUser;
        _hotelRepository = hotelRepository;
        _logger = logger;
    }

    public async Task<ErrorOr<List<Guid>>> Handle(UpdateHotelAmenitiesCommand request, CancellationToken cancellationToken)
    {
        // 1. Validate current user
        if (!_currentUser.IsAuthenticated || string.IsNullOrEmpty(_currentUser.Id))
        {
            return Error.Unauthorized(
                code: "User.Unauthorized",
                description: "User must be authenticated to create a hotel.");
        }

        // 2. Get user from database
        var user = await _userRepository.GetByIdentityIdAsync(_currentUser.Id, cancellationToken);
        if (user is null)
        {
            return Error.NotFound(
                code: "User.NotFound",
                description: "User profile not found. Please complete registration first.");
        }
        _logger.LogWarning($"Updating hotel {request.HotelId} for user {user.Id}");
        var hotel = await _hotelRepository.GetByIdAsync(
            (HotelId)request.HotelId, 
            cancellationToken);
    
        if (hotel is null)
            return Error.NotFound(
                code: "Hotel.NotFound", 
                description: $"We couldn't find a hotel with this id {request.HotelId}.");

        // 4. Verify ownership
        if (hotel.OwnerId != user.Id)
        {
            return Error.Forbidden(
                code: "Hotel.Forbidden",
                description: "You don't have permission to modify this hotel.");
        }
        var amenityDefinitions = await _referenceRepository
            .GetActiveHotelAmenitiesAsync(cancellationToken);
        var requestedAmenityIds = request.AmenitiesIds.ToHashSet();
        var validDefinitions = amenityDefinitions
            .Where(a => requestedAmenityIds.Contains(a.Id.Value) && a.IsActive)
            .ToList();
        if (validDefinitions.Count != request.AmenitiesIds.Count)
        {
            return Error.Validation(
                code: "Hotel.InvalidAmenities",
                description: "One or more amenity IDs are invalid or inactive.");
        }
        
        var selectedAmenities = validDefinitions
            .Select(def => HotelSelectedAmenity.Create(
                (HotelAmenityDefinitionId)def.Id.Value,
                Upcharge.CreateFlat(Money.Zero(Currency.MAD)).Value
            ))
            .ToList();
        
        var result = hotel.SyncAmenities(selectedAmenities);
    
        if (result.IsError)
            return result.Errors;
        return selectedAmenities
            .Select(a => a.AmenityDefinitionId.Value)
            .ToList();
    }
}