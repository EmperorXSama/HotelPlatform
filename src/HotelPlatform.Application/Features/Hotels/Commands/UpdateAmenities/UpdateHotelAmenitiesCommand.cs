namespace HotelPlatform.Application.Features.Hotels.Commands.UpdateAmenities;

public record UpdateHotelAmenitiesCommand(Guid HotelId, List<Guid> AmenitiesIds) : ICommand<ErrorOr<List<Guid>>>;