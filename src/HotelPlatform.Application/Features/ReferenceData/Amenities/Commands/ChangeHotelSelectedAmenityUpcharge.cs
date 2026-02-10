using HotelPlatform.Domain.Common.ValueObjects;

namespace HotelPlatform.Application.Features.ReferenceData.Amenities.Commands;

public record ChangeHotelSelectedAmenityUpcharge(Guid HotelId,Guid AmenityId, int UpchargeType ,decimal Amount, string? CurrencyCode) 
    : ICommand<ErrorOr<Success>>;