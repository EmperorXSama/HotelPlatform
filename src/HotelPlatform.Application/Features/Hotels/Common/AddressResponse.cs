namespace HotelPlatform.Application.Features.Hotels.Common;

public sealed record AddressResponse(
    string Street,
    string City,
    string Country,
    string? PostalCode);