namespace HotelPlatform.Domain.Common.StronglyTypedIds;

public readonly record struct RoomAmenityDefinitionId(Guid Value): IStronglyTypedId<Guid>
{
    public static RoomAmenityDefinitionId New() => new(Guid.NewGuid());
    public static RoomAmenityDefinitionId Empty => new(Guid.Empty);
    
    public override string ToString() => Value.ToString();
    
    public static implicit operator Guid(RoomAmenityDefinitionId id) => id.Value;
    public static explicit operator RoomAmenityDefinitionId(Guid id) => new(id);
}