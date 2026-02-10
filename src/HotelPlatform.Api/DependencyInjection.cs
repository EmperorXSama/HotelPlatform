using System.Security.Claims;
using Azure.Identity;
using HotelPlatform.Api.Authentication;
using HotelPlatform.Api.Services;
using HotelPlatform.Application;
using HotelPlatform.Application.Common.Interfaces.Storage;
using HotelPlatform.Application.Common.Settings;
using HotelPlatform.Infrastructure;
using HotelPlatform.Infrastructure.Storage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace HotelPlatform.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddApiServices(
        this IServiceCollection services,
        IConfiguration configuration) =>
        services
            .AddApiServices()
            .AddSettings(configuration)
            .AddApplicationServices(configuration)
            .AddInfrastructureServices(configuration)
            .AddAuthenticationServices(configuration)
            .AddAuthorizationServices();

    // Api/DependencyInjection.cs
    private static IServiceCollection AddSettings(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<DatabaseSettings>()
            .Bind(configuration.GetSection(DatabaseSettings.SectionName))
            .PostConfigure<IConfiguration>((settings, config) =>
            {
                // Only needed for Database because Aspire injects it as ConnectionStrings:hoteldb
                if (string.IsNullOrEmpty(settings.ConnectionString))
                {
                    settings.ConnectionString = config.GetConnectionString("hoteldb") ?? string.Empty;
                }
            })
            .ValidateDataAnnotations()
            .ValidateOnStart();

        // Storage - simple bind, no fallback needed
        services.AddOptions<StorageSettings>()
            .Bind(configuration.GetSection(StorageSettings.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<KeycloakSettings>()
            .Bind(configuration.GetSection(KeycloakSettings.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }

    private static IServiceCollection AddAuthenticationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var keycloakSettings = configuration
            .GetSection(KeycloakSettings.SectionName)
            .Get<KeycloakSettings>()!;

        // Register JWT events handler
        services.AddScoped<JwtEventsHandler>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = keycloakSettings.Authority;
                options.Audience = keycloakSettings.Audience;
                options.RequireHttpsMetadata = false; // TODO: Set true in production

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.FromSeconds(30),
                    NameClaimType = "preferred_username",
                    RoleClaimType = ClaimTypes.Role
                };

                // Use custom events handler
                options.EventsType = typeof(JwtEventsHandler);
            });

        return services;
    }

    private static IServiceCollection AddAuthorizationServices(
        this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .AddPolicy("SuperAdmin", policy => policy.RequireRole("super-admin"))
            .AddPolicy("HotelOwner", policy => policy.RequireRole("hotel-owner", "super-admin"))
            .AddPolicy("User", policy => policy.RequireRole("user"));

        return services;
    }

    private static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<IFileUrlResolver, FileUrlResolver>();
        return services;
    }

}