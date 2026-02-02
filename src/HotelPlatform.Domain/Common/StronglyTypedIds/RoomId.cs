namespace HotelPlatform.Domain.Common.StronglyTypedIds;

public readonly record struct RoomId(Guid Value): IStronglyTypedId<Guid>
{
    public static RoomId New()=> new(Guid.NewGuid());
    public static RoomId Empty => new(Guid.Empty);
    
    public override string ToString() => Value.ToString();
    
    public static implicit operator Guid(RoomId id) => id.Value;
    public static explicit operator RoomId(Guid id) => new(id);
}