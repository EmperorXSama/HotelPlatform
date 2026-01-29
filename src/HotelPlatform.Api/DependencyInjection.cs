using System.Security.Claims;
using HotelPlatform.Api.Authentication;
using HotelPlatform.Application;
using HotelPlatform.Application.Common.Settings;
using HotelPlatform.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace HotelPlatform.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddApiServices(
        this IServiceCollection services,
        IConfiguration configuration) =>
        services
            .AddSettings(configuration)
            .AddApplicationServices(configuration)
            .AddInfrastructureServices(configuration)
            .AddAuthenticationServices(configuration)
            .AddAuthorizationServices();

    private static IServiceCollection AddSettings(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<DatabaseSettings>()
            .Bind(configuration.GetSection(DatabaseSettings.SectionName))
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
}