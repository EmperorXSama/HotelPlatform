namespace HotelPlatform.Domain.Common.StronglyTypedIds;

public readonly record struct  UserId(Guid Value): IStronglyTypedId<Guid>
{
    public static UserId New() => new(Guid.NewGuid());
    public static UserId Empty => new(Guid.Empty);
    public override string ToString() => Value.ToString();
    public static implicit operator Guid(UserId id) => id.Value;
    public static explicit operator UserId(Guid id) => new(id);
}