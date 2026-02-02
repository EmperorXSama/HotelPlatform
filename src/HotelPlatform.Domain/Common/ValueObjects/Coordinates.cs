using HotelPlatform.Domain.Common.Errors;

namespace HotelPlatform.Domain.Common.ValueObjects;

public sealed record Coordinates
{
    public double Latitude { get; }
    public double Longitude { get; }
    private Coordinates() { }
    private Coordinates(double latitude, double longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }
    
    public static ErrorOr<Coordinates> Create(double latitude, double longitude)
    {
        if (latitude is < -90 or > 90)
            return DomainErrors.Coordinates.InvalidLatitude;

        if (longitude is < -180 or > 180)
            return DomainErrors.Coordinates.InvalidLongitude;

        return new Coordinates(latitude, longitude);
    }

    public override string ToString() => $"({Latitude:F6}, {Longitude:F6})";
}