using HotelPlatform.Application;
using HotelPlatform.Application.Common.Settings;
using HotelPlatform.Infrastructure;

namespace HotelPlatform.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddApiServices(this IServiceCollection services , IConfiguration configuration) =>
        services.AddSettings(configuration)
            .AddApplicationServices(configuration)
            .AddInfrastructureServices(configuration);
    
    
    
    public static IServiceCollection AddSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<DatabaseSettings>()
            .Bind(configuration.GetSection(DatabaseSettings.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        return services;
    }
}