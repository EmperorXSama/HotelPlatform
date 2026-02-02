namespace HotelPlatform.Domain.Common.StronglyTypedIds;

public readonly record struct RatingId(Guid Value): IStronglyTypedId<Guid>
{
    public static RatingId New() => new(Guid.NewGuid());
    public static RatingId Empty()=> new(Guid.Empty);
    
    public override string ToString() => Value.ToString();
    
    public static implicit operator RatingId(Guid id) => new(id);
    public static explicit operator Guid(RatingId id) => id.Value;
}