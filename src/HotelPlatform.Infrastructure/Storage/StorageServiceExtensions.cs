// Infrastructure/Storage/StorageServiceExtensions.cs
using Azure.Identity;
using HotelPlatform.Application.Common.Interfaces;
using HotelPlatform.Application.Common.Interfaces.Storage;
using HotelPlatform.Application.Common.Settings;
using HotelPlatform.Infrastructure.Storage.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HotelPlatform.Infrastructure.Storage;

public static class StorageServiceExtensions
{
    public static IServiceCollection AddStorageServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Bind settings
        services.Configure<StorageSettings>(
            configuration.GetSection(StorageSettings.SectionName));

        // Register validators
        services.AddScoped<IFileValidator, FileValidator>();

        // Register storage providers
        services.AddScoped<IStorageProvider, AzureBlobStorageProvider>();
        services.AddScoped<IStorageProvider, LocalFileStorageProvider>();

        // Register main storage service
        services.AddScoped<IFileStorageService, FileStorageService>();

        return services;
    }
}