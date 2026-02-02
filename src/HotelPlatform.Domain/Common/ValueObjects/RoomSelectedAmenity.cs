using HotelPlatform.Domain.Common.StronglyTypedIds;

namespace HotelPlatform.Domain.Common.ValueObjects;


public sealed record RoomSelectedAmenity
{
    public RoomAmenityDefinitionId AmenityDefinitionId { get; }
    public Upcharge Upcharge { get; init; }

    private RoomSelectedAmenity(
        RoomAmenityDefinitionId amenityDefinitionId,
        Upcharge upcharge)
    {
        AmenityDefinitionId = amenityDefinitionId;
        Upcharge = upcharge;
    }
    private RoomSelectedAmenity()
    { }
    public static RoomSelectedAmenity Create(
        RoomAmenityDefinitionId amenityDefinitionId,
        Upcharge upcharge)
    {
        return new RoomSelectedAmenity(amenityDefinitionId, upcharge);
    }

    public RoomSelectedAmenity WithUpcharge(Upcharge upcharge) => 
        this with { Upcharge = upcharge };
}