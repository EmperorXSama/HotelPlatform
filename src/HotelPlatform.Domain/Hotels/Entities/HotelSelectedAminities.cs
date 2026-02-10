using HotelPlatform.Domain.Common.StronglyTypedIds;
using HotelPlatform.Domain.Common.ValueObjects;

namespace HotelPlatform.Domain.Hotels.Entities;

public sealed record HotelSelectedAmenity
{
    public HotelAmenityDefinitionId AmenityDefinitionId { get; }
    public Upcharge Upcharge { get; set; }
    
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