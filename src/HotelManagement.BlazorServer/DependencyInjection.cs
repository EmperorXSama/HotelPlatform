using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using HotelManagement.BlazorServer.Http;
using HotelManagement.BlazorServer.Services;
using HotelManagement.BlazorServer.Settings;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace HotelManagement.BlazorServer;

public static class DependencyInjection
{
    
    public static IServiceCollection AddBlazorServerServices(this IServiceCollection services, IConfiguration configuration)=>
    services.AddAuthenticationServices(configuration)
        .AddApiClient(configuration);
    public static IServiceCollection AddAuthenticationServices(this IServiceCollection services, IConfiguration configuration)
    {
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

        var keycloakSettings = configuration
            .GetSection(WebKeycloakSettings.SectionName)
            .Get<WebKeycloakSettings>()!;

        services.AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
        })
        .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
        {
            options.Cookie.Name = "HotelPlatform.Auth";
            options.Cookie.SameSite = SameSiteMode.Lax;
            options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        })
        .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
        {
            options.Authority = keycloakSettings.Authority;
            options.ClientId = keycloakSettings.ClientId;
            options.ClientSecret = keycloakSettings.ClientSecret;

            options.ResponseType = OpenIdConnectResponseType.Code;
            options.SaveTokens = true;
            options.GetClaimsFromUserInfoEndpoint = true;

            options.RequireHttpsMetadata = false; // TODO: Set true in production

            // Scopes to request
            options.Scope.Clear();
            options.Scope.Add("openid");
            options.Scope.Add("profile");
            options.Scope.Add("email");
            options.Scope.Add("roles"); // Add roles scope

            // Map Keycloak claims
            options.TokenValidationParameters.NameClaimType = "preferred_username";
            options.TokenValidationParameters.RoleClaimType = ClaimTypes.Role;

            // Map claims properly
            options.ClaimActions.MapJsonKey(ClaimTypes.Role, "roles");
            options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
            options.ClaimActions.MapJsonKey(ClaimTypes.Name, "preferred_username");
            options.ClaimActions.MapJsonKey("email_verified", "email_verified");

            // Handle roles claim
            options.Events = new OpenIdConnectEvents
            {
                OnTokenValidated = context =>
                {
                    MapKeycloakRolesToClaims(context);
                    return Task.CompletedTask;
                }
            };
        });

        services.AddAuthorizationBuilder()
            .AddPolicy("SuperAdmin", policy => policy.RequireRole("super-admin"))
            .AddPolicy("HotelOwner", policy => policy.RequireRole("hotel-owner", "super-admin"))
            .AddPolicy("User", policy => policy.RequireRole("user"));

        return services;
    }
    
    public static IServiceCollection AddApiClient(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register settings
        services.Configure<WebKeycloakSettings>(
            configuration.GetSection(WebKeycloakSettings.SectionName));

        // Register HttpContextAccessor
        services.AddHttpContextAccessor();

        // Register token service
        services.AddScoped<ITokenService, TokenService>();

        // Register the handler
        services.AddScoped<TokenForwardingHandler>();

        var apiBaseUrl = configuration["ApiSettings:BaseUrl"]
                         ?? throw new InvalidOperationException("API base URL not configured");

        services.AddHotelApiServices(apiBaseUrl);

      
        return services;
    }
    static void MapKeycloakRolesToClaims(TokenValidatedContext context)
    {
        if (context.Principal?.Identity is not ClaimsIdentity identity)
            return;

        // Remove existing role claims to avoid duplicates
        var existingRoleClaims = identity.FindAll("roles").ToList();
        foreach (var claim in existingRoleClaims)
        {
            identity.RemoveClaim(claim);
        }

        // Get all roles from realm_access
        var realmAccessClaim = identity.FindFirst(c => c.Type == "realm_access");
        if (realmAccessClaim != null)
        {
            try
            {
                var realmAccess = System.Text.Json.JsonDocument.Parse(realmAccessClaim.Value);
                if (realmAccess.RootElement.TryGetProperty("roles", out var rolesElement))
                {
                    foreach (var role in rolesElement.EnumerateArray())
                    {
                        var roleValue = role.GetString();
                        if (!string.IsNullOrEmpty(roleValue))
                        {
                            identity.AddClaim(new Claim(ClaimTypes.Role, roleValue));
                        }
                    }
                }
            }
            catch
            {
                // Parsing failed, continue
            }
        }

        // Also check for direct roles claim (which Keycloak mapper might add)
        var directRolesClaims = context.Principal.FindAll("roles").ToList();
        foreach (var roleClaim in directRolesClaims)
        {
            // Add as standard Role claim if not already added
            if (!identity.HasClaim(ClaimTypes.Role, roleClaim.Value))
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, roleClaim.Value));
            }
        }
    }
}