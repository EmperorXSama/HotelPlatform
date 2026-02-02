using HotelPlatform.Domain.Common.StronglyTypedIds;

namespace HotelPlatform.Domain.Common.ValueObjects;

public sealed record HotelSelectedAmenity
{
    public HotelAmenityDefinitionId AmenityDefinitionId { get; }
    public Upcharge Upcharge { get; init; }
    
    private HotelSelectedAmenity()
    { }
    private HotelSelectedAmenity(
        HotelAmenityDefinitionId amenityDefinitionId,
        Upcharge upcharge)
    {
        AmenityDefinitionId = amenityDefinitionId;
        Upcharge = upcharge;
    }
    public static HotelSelectedAmenity Create(
        HotelAmenityDefinitionId amenityDefinitionId,
        Upcharge upcharge)
    {
        return new HotelSelectedAmenity(amenityDefinitionId, upcharge);
    }
    public HotelSelectedAmenity WithUpcharge(Upcharge upcharge) => 
        this with { Upcharge = upcharge };
}