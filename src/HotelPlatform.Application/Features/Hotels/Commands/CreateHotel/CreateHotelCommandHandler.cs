using HotelPlatform.Application.Common.Interfaces;
using HotelPlatform.Application.Common.Interfaces.Repositories;
using HotelPlatform.Application.Common.Interfaces.Storage;
using HotelPlatform.Domain.Common.ValueObjects;
using HotelPlatform.Domain.Hotels.Entities;


namespace HotelPlatform.Application.Features.Hotels.Commands.CreateHotel;

public sealed class CreateHotelCommandHandler
    : IRequestHandler<CreateHotelCommand, ErrorOr<CreateHotelResult>>
{
    private readonly IHotelRepository _hotelRepository;
    private readonly IUserRepository _userRepository;
    private readonly IStoredFileRepository _storedFileRepository;
    private readonly IReferenceDataRepository _referenceDataRepository;
    private readonly ICurrentUser _currentUser;

    public CreateHotelCommandHandler(
        IHotelRepository hotelRepository,
        IUserRepository userRepository,
        IStoredFileRepository storedFileRepository,
        ICurrentUser currentUser, IReferenceDataRepository referenceDataRepository)
    {
        _hotelRepository = hotelRepository;
        _userRepository = userRepository;
        _storedFileRepository = storedFileRepository;
        _currentUser = currentUser;
        _referenceDataRepository = referenceDataRepository;
    }

    public async Task<ErrorOr<CreateHotelResult>> Handle(
        CreateHotelCommand request,
        CancellationToken cancellationToken)
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

        // 3. Validate all pictures exist and belong to the user
        var pictureValidationResult = await ValidatePicturesAsync(
            request.Pictures,
            user.Id,
            cancellationToken);

        if (pictureValidationResult.IsError)
            return pictureValidationResult.Errors;

        // 4. Create the hotel
        var hotelResult = Hotel.Create(
            user.Id,
            request.Name,
            request.Description);

        if (hotelResult.IsError)
            return hotelResult.Errors;

        var hotel = hotelResult.Value;

        // 5. Add address if provided
        if (request.Address is not null)
        {
            var addressResult = CreateAddress(request.Address);
            if (addressResult.IsError)
                return addressResult.Errors;

            var updateAddressResult = hotel.UpdateAddress(addressResult.Value);
            if (updateAddressResult.IsError)
                return updateAddressResult.Errors;
        }
        if (request.Amenities is not null && request.Amenities.Any())
        {
            var amenityIds = request.Amenities
                .Select(a => (HotelAmenityDefinitionId)a.AmenityDefinitionId)
                .ToList();
            var existingAmenities = await _referenceDataRepository
                .GetHotelAmenitiesByIdsAsync(amenityIds, cancellationToken);
            var missingIds = amenityIds
                .Where(id => !existingAmenities.Any(a => a.Id == id))
                .ToList();
            if (missingIds.Any())
            {
                return Error.NotFound(
                    code: "Amenity.NotFound",
                    description: $"Amenity definitions not found: {string.Join(", ", missingIds)}");
            }
        }
        if (request.Amenities is not null)
        {
            foreach (var amenityDto in request.Amenities)
            {
                // Create upcharge based on type
                ErrorOr<Upcharge> upchargeResult = amenityDto.UpchargeType == 0
                    ? Upcharge.CreateFlat(amenityDto.UpchargeAmount, Currency.FromCode(amenityDto.Currency!).Value)
                    : Upcharge.CreatePercentage(amenityDto.UpchargeAmount);
                
                if (upchargeResult.IsError)
                    return upchargeResult.Errors;
                
                var selectedAmenity = HotelSelectedAmenity.Create(
                    (HotelAmenityDefinitionId)amenityDto.AmenityDefinitionId,
                    upchargeResult.Value
                );
                var addAmenityResult = hotel.AddAmenity(selectedAmenity);
                if (addAmenityResult.IsError)
                    return addAmenityResult.Errors;
            }
        }
        // 6. Add pictures
        string? mainPictureUrl = null;
        foreach (var pictureDto in request.Pictures.OrderByDescending(p => p.IsMain))
        {
            var storedFileId = new StoredFileId(pictureDto.StoredFileId);
            var picture = HotelPicture.Create(
                storedFileId,
                pictureDto.AltText,
                pictureDto.IsMain,
                request.Pictures.IndexOf(pictureDto));

            var addPictureResult = hotel.AddPicture(picture);
            if (addPictureResult.IsError)
                return addPictureResult.Errors;

            // Get main picture URL for response
            if (pictureDto.IsMain)
            {
                var storedFile = await _storedFileRepository.GetByIdAsync(storedFileId, cancellationToken);
                mainPictureUrl = storedFile?.Url;
            }
        }
        
        await _hotelRepository.AddAsync(hotel, cancellationToken);

        // 8. Return result
        return new CreateHotelResult(
            hotel.Id.Value);
    }

    private async Task<ErrorOr<Success>> ValidatePicturesAsync(
        List<CreateHotelPictureDto> pictures,
        UserId userId,
        CancellationToken cancellationToken)
    {
        var errors = new List<Error>();

        foreach (var picture in pictures)
        {
            var storedFileId = new StoredFileId(picture.StoredFileId);
            var storedFile = await _storedFileRepository.GetByIdAsync(storedFileId, cancellationToken);

            if (storedFile is null)
            {
                errors.Add(Error.NotFound(
                    code: "StoredFile.NotFound",
                    description: $"Picture with ID '{picture.StoredFileId}' was not found."));
                continue;
            }

            if (storedFile.OwnerId != userId)
            {
                errors.Add(Error.Forbidden(
                    code: "StoredFile.NotOwned",
                    description: $"Picture with ID '{picture.StoredFileId}' does not belong to you."));
            }
        }

        return errors.Count > 0 ? errors : Result.Success;
    }

    private static ErrorOr<Address> CreateAddress(CreateHotelAddressDto dto)
    {
        Coordinates? coordinates = null;

        if (dto.Latitude.HasValue && dto.Longitude.HasValue)
        {
            var coordResult = Coordinates.Create(dto.Latitude.Value, dto.Longitude.Value);
            if (coordResult.IsError)
                return coordResult.Errors;

            coordinates = coordResult.Value;
        }

        return Address.Create(
            dto.Street,
            dto.City,
            dto.Country,
            dto.PostalCode,
            coordinates);
    }
}