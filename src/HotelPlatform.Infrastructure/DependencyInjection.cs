using HotelPlatform.Application.Common.Interfaces;
using HotelPlatform.Application.Common.Interfaces.Repositories;
using HotelPlatform.Application.Common.Interfaces.Storage;
using HotelPlatform.Application.Common.Settings;
using HotelPlatform.Infrastructure.Data;
using HotelPlatform.Infrastructure.Data.Interceptors;
using HotelPlatform.Infrastructure.Identity;
using HotelPlatform.Infrastructure.Repositories;
using HotelPlatform.Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace HotelPlatform.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration) =>
        services
            .AddDatabaseService(configuration)
            .AddRepositories()
            .AddStorageServices(configuration);


    public static IServiceCollection AddDatabaseService(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            var settings = sp.GetRequiredService<IOptions<DatabaseSettings>>().Value;
        
            options.UseNpgsql(settings.ConnectionString, npgOptions =>
            {
                npgOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
            });

            var interceptor = sp.GetService<DispatchDomainEventsInterceptor>();
            if (interceptor is not null)
            {
                options.AddInterceptors(interceptor);
            }
        });
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());
        
        return services;
    }
    
    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IHotelRepository, HotelRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRatingRepository, RatingRepository>();
        services.AddScoped<IStoredFileRepository, StoredFileRepository>();
        services.AddScoped<IReferenceDataRepository, ReferenceDataRepository>();

        return services;
    }
}