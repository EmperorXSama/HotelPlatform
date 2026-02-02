namespace HotelPlatform.Domain.Common.StronglyTypedIds;

public readonly record struct StoredFileId(Guid Value): IStronglyTypedId<Guid>
{
    public static StoredFileId New() => new(Guid.NewGuid());
    public static StoredFileId Empty => new(Guid.Empty);

    public override string ToString() => Value.ToString();

    public static implicit operator Guid(StoredFileId id) => id.Value;
    public static explicit operator StoredFileId(Guid id) => new(id);
}