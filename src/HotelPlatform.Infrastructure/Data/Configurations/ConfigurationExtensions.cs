using HotelPlatform.Domain.Abstractions;
using HotelPlatform.Infrastructure.Data.Converters;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelPlatform.Infrastructure.Data.Configurations;

public static class ConfigurationExtensions
{
    public static PropertyBuilder<TId> HasStronglyTypedId<TId>(this PropertyBuilder<TId> builder)
        where TId : struct, IStronglyTypedId<Guid>
    {
        return builder.HasConversion(new StronglyTypedIdConverter<TId>());
    }
}