using HotelPlatform.Application.Common.Interfaces;
using HotelPlatform.Application.Common.Interfaces.Repositories;
using HotelPlatform.Application.Common.Interfaces.Storage;
using HotelPlatform.Domain.Common.ValueObjects;


namespace HotelPlatform.Application.Features.Hotels.Commands.CreateHotel;

public sealed class CreateHotelCommandHandler
    : IRequestHandler<CreateHotelCommand, ErrorOr<CreateHotelResult>>
{
    private readonly IHotelRepository _hotelRepository;
    private readonly IUserRepository _userRepository;
    private readonly IStoredFileRepository _storedFileRepository;
    private readonly ICurrentUser _currentUser;

    public CreateHotelCommandHandler(
        IHotelRepository hotelRepository,
        IUserRepository userRepository,
        IStoredFileRepository storedFileRepository,
        ICurrentUser currentUser,
        IUnitOfWork unitOfWork)
    {
        _hotelRepository = hotelRepository;
        _userRepository = userRepository;
        _storedFileRepository = storedFileRepository;
        _currentUser = currentUser;
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

        // 7. Save to database
        await _hotelRepository.AddAsync(hotel, cancellationToken);

        // 8. Return result
        return new CreateHotelResult(
            hotel.Id.Value,
            hotel.Name,
            hotel.Description,
            hotel.Status.ToString(),
            mainPictureUrl,
            hotel.Pictures.Count,
            hotel.CreatedAt);
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