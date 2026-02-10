namespace HotelPlatform.Domain.Common.StronglyTypedIds;

public record struct HotelId(Guid Value): IStronglyTypedId<Guid>
{
    public static HotelId New() => new(Guid.NewGuid());
    public static HotelId Empty => new(Guid.Empty);
    
    public override string ToString() => Value.ToString();
    
    public static implicit operator Guid(HotelId id) => id.Value;
    public static explicit operator HotelId(Guid id) => new(id);
    
}