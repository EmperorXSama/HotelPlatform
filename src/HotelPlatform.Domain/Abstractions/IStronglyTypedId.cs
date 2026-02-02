namespace HotelPlatform.Domain.Abstractions;

public interface IStronglyTypedId<out T>  where T : notnull
{
    T Value { get; }
}