namespace HotelPlatform.Domain.Common.StronglyTypedIds;


public readonly record struct RoomTypeDefinitionId(Guid Value): IStronglyTypedId<Guid>
{
    public static RoomTypeDefinitionId New() => new(Guid.NewGuid());
    public static RoomTypeDefinitionId Empty => new(Guid.Empty);
    
    public override string ToString() => Value.ToString();
    
    public static implicit operator Guid(RoomTypeDefinitionId id) => id.Value;
    public static explicit operator RoomTypeDefinitionId(Guid id) => new(id);
}