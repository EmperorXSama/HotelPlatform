using System.Linq.Expressions;
using HotelPlatform.Domain.Abstractions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace HotelPlatform.Infrastructure.Data.Converters;
public class StronglyTypedIdConverter<TId> : ValueConverter<TId, Guid>
    where TId : struct, IStronglyTypedId<Guid>
{
    public StronglyTypedIdConverter()
        : base(
            id => id.Value,
            guid => CreateId(guid))
    {
    }

    private static TId CreateId(Guid guid)
    {
        return (TId)Activator.CreateInstance(typeof(TId), guid)!;
    }
}