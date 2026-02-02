using HotelPlatform.Domain.Common.Errors;

namespace HotelPlatform.Domain.Common.ValueObjects;

public sealed record  Address
{
    public string Street { get; }
    public string City { get; }
    public string Country { get; }
    public string? PostalCode { get; }
    public Coordinates? Coordinates { get; }
    private Address() { }
    private Address(
        string street,
        string city,
        string country,
        string? postalCode,
        Coordinates? coordinates)
    {
        Street = street;
        City = city;
        Country = country;
        PostalCode = postalCode;
        Coordinates = coordinates;
    }

    public static ErrorOr<Address> Create(
        string street,
        string city,
        string country,
        string? postalCode = null,
        Coordinates? coordinates = null)
    {
        var errors = new List<Error>();

        if (string.IsNullOrWhiteSpace(street))
            errors.Add(DomainErrors.Address.StreetRequired);

        if (string.IsNullOrWhiteSpace(city))
            errors.Add(DomainErrors.Address.CityRequired);

        if (string.IsNullOrWhiteSpace(country))
            errors.Add(DomainErrors.Address.CountryRequired);

        if (errors.Count > 0)
            return errors;

        return new Address(
            street.Trim(),
            city.Trim(),
            country.Trim(),
            postalCode?.Trim(),
            coordinates);
    }
    
    public Address WithCoordinates(Coordinates coordinates) =>
        new(Street, City, Country, PostalCode, coordinates);
    public override string ToString() =>
        string.IsNullOrWhiteSpace(PostalCode)
            ? $"{Street}, {City}, {Country}"
            : $"{Street}, {City}, {PostalCode}, {Country}";
}