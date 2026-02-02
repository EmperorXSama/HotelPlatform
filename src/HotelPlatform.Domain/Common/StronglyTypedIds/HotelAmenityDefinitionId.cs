

namespace HotelPlatform.Domain.Common.StronglyTypedIds;

public readonly record struct HotelAmenityDefinitionId(Guid Value): IStronglyTypedId<Guid>
{
    public static HotelAmenityDefinitionId New() => new(Guid.NewGuid());
    public static HotelAmenityDefinitionId Empty => new(Guid.Empty);
    
    public override string ToString() => Value.ToString();
    
    public static implicit operator Guid(HotelAmenityDefinitionId id) => id.Value;
    public static explicit operator HotelAmenityDefinitionId(Guid id) => new(id);
}