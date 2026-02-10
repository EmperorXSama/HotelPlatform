using HotelPlatform.Application.Common.Interfaces;
using HotelPlatform.Application.Common.Interfaces.Repositories;
using HotelPlatform.Domain.Common.Errors;
using HotelPlatform.Domain.Common.ValueObjects;

namespace HotelPlatform.Application.Features.ReferenceData.Amenities.Commands;

public class
    ChangeHotelSelectedAmenityCommandHandler : IRequestHandler<ChangeHotelSelectedAmenityUpcharge, ErrorOr<Success>>
{

    private readonly ICurrentUser _currentUser;
    private readonly IUserRepository _userRepository;
    private readonly IHotelRepository _hotelRepository;
    private readonly IReferenceDataRepository _referenceDataRepository;
    public ChangeHotelSelectedAmenityCommandHandler(ICurrentUser currentUser, IUserRepository userRepository,
        IHotelRepository hotelRepository, IReferenceDataRepository referenceDataRepository)
    {
        _currentUser = currentUser;
        _userRepository = userRepository;
        _hotelRepository = hotelRepository;
        _referenceDataRepository = referenceDataRepository;
    }

    public async Task<ErrorOr<Success>> Handle(ChangeHotelSelectedAmenityUpcharge request,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated || string.IsNullOrEmpty(_currentUser.Id))
        {
            return Error.Unauthorized(
                code: "User.Unauthorized",
                description: "User must be authenticated to create a hotel.");
        }

        var user = await _userRepository.GetByIdentityIdAsync(_currentUser.Id, cancellationToken);
        if (user is null)
        {
            return Error.NotFound(
                code: "User.NotFound",
                description: "User profile not found. Please complete registration first.");
        }

      
  // 1. Get the hotel with amenities
        var hotelId = (HotelId)(request.HotelId);
        var hotel = await _hotelRepository.GetByIdWithAmenitiesAsync(
            hotelId, 
            cancellationToken);

        if (hotel is null)
            return DomainErrors.Hotel.NotFound(request.HotelId);
        if (hotel.OwnerId != user.Id)
            return Error.Validation("Hotel owner does not match", "Hotel owner does not match");
        // 2. Verify the amenity definition exists and is active
        var amenityDefinitionId = (HotelAmenityDefinitionId)(request.AmenityId);
        var amenityDefinition = await _referenceDataRepository
            .GetHotelAmenityByIdAsync(amenityDefinitionId, cancellationToken);

        if (amenityDefinition is null)
            return Error.NotFound(
                code: "AmenityDefinition.NotFound",
                description: $"Amenity definition with id '{request.AmenityId}' was not found.");

        if (!amenityDefinition.IsActive)
            return Error.Validation(
                code: "AmenityDefinition.NotActive",
                description: "Cannot update upcharge for an inactive amenity.");

        // 3. Create the new upcharge
        var upchargeType = (UpchargeType)request.UpchargeType;
        ErrorOr<Upcharge> upchargeResult;

        if (upchargeType == UpchargeType.Flat)
        {
            if (string.IsNullOrWhiteSpace(request.CurrencyCode))
                return Error.Validation(
                    code: "Upcharge.CurrencyRequired",
                    description: "Currency code is required for flat upcharge type.");

            var currencyResult = Currency.FromCode(request.CurrencyCode);
            if (currencyResult.IsError)
                return currencyResult.Errors;

            upchargeResult = Upcharge.CreateFlat(request.Amount, currencyResult.Value);
        }
        else if (upchargeType == UpchargeType.PercentagePerNight)
        {
            upchargeResult = Upcharge.CreatePercentage(request.Amount);
        }
        else
        {
            return Error.Validation(
                code: "Upcharge.InvalidType",
                description: $"Invalid upcharge type: {request.UpchargeType}");
        }

        if (upchargeResult.IsError)
            return upchargeResult.Errors;

        // 4. Update the amenity upcharge on the hotel
        var updateResult = hotel.UpdateAmenityUpcharge(
            amenityDefinitionId, 
            upchargeResult.Value);

        if (updateResult.IsError)
            return updateResult.Errors;
        return Result.Success;
    }
}