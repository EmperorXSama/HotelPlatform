using HotelPlatform.Application.Common.Interfaces;
using HotelPlatform.Application.Common.Settings;
using HotelPlatform.Infrastructure.Data;
using HotelPlatform.Infrastructure.Identity;
using HotelPlatform.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HotelPlatform.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration) =>
        services
            .AddDatabaseService(configuration);


    public static IServiceCollection AddDatabaseService(this IServiceCollection services , IConfiguration configuration)
    {
        
        var databaseSettings = configuration.GetSection(DatabaseSettings.SectionName)
            .Get<DatabaseSettings>();

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            var connectionString = databaseSettings!.ConnectionString;
            options.UseNpgsql(connectionString, npgOptions =>
            {
                npgOptions.EnableRetryOnFailure(
                    maxRetryCount: databaseSettings.MaxRetryCount,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorCodesToAdd: null);
                npgOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
            });
        });
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());
        services.AddScoped(typeof(IRepository<>),typeof(Repository<>));
        
        return services;
    }
}