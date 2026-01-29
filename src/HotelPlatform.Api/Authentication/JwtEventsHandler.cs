using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace HotelPlatform.Api.Authentication;

public class JwtEventsHandler : JwtBearerEvents
{
    private readonly ILogger<JwtEventsHandler> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public JwtEventsHandler(ILogger<JwtEventsHandler> logger)
    {
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public override Task TokenValidated(TokenValidatedContext context)
    {
        if (context.Principal?.Identity is ClaimsIdentity identity)
        {
            MapKeycloakRolesToClaims(identity);
        }

        return Task.CompletedTask;
    }

    public override Task AuthenticationFailed(AuthenticationFailedContext context)
    {
        _logger.LogWarning(
            "Authentication failed for {Path}: {Error}",
            context.Request.Path,
            context.Exception.Message);

        return Task.CompletedTask;
    }

    public override async Task Challenge(JwtBearerChallengeContext context)
    {
        context.HandleResponse();

        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.ContentType = "application/json";

        var errorResponse = new
        {
            error = "Unauthorized",
            message = GetFriendlyErrorMessage(context),
            timestamp = DateTime.UtcNow
        };

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(errorResponse, _jsonOptions));
    }

    public override async Task Forbidden(ForbiddenContext context)
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        context.Response.ContentType = "application/json";

        var errorResponse = new
        {
            error = "Forbidden",
            message = "You don't have permission to access this resource",
            timestamp = DateTime.UtcNow
        };

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(errorResponse, _jsonOptions));
    }

    private void MapKeycloakRolesToClaims(ClaimsIdentity identity)
    {
        var rolesToAdd = new HashSet<string>();

        // 1. Parse realm_access.roles (nested JSON)
        var realmAccessClaim = identity.FindFirst("realm_access");
        if (realmAccessClaim is not null)
        {
            try
            {
                using var doc = JsonDocument.Parse(realmAccessClaim.Value);
                if (doc.RootElement.TryGetProperty("roles", out var rolesElement))
                {
                    foreach (var role in rolesElement.EnumerateArray())
                    {
                        var roleValue = role.GetString();
                        if (!string.IsNullOrEmpty(roleValue))
                        {
                            rolesToAdd.Add(roleValue);
                        }
                    }
                }
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(ex, "Failed to parse realm_access claim");
            }
        }

        // 2. Parse flat "roles" claim (could be JSON array or multiple claims)
        var rolesClaims = identity.FindAll("roles").ToList();
        foreach (var claim in rolesClaims)
        {
            if (claim.Value.StartsWith("["))
            {
                // JSON array
                try
                {
                    var roles = JsonSerializer.Deserialize<string[]>(claim.Value);
                    if (roles is not null)
                    {
                        foreach (var role in roles)
                        {
                            rolesToAdd.Add(role);
                        }
                    }
                }
                catch (JsonException ex)
                {
                    _logger.LogWarning(ex, "Failed to parse roles claim as JSON array");
                }
            }
            else
            {
                // Single value
                rolesToAdd.Add(claim.Value);
            }
        }

        // 3. Add all roles as standard ClaimTypes.Role claims
        foreach (var role in rolesToAdd)
        {
            // Check if this role claim already exists
            if (!identity.HasClaim(ClaimTypes.Role, role))
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, role));
                _logger.LogDebug("Added role claim: {Role}", role);
            }
        }

        _logger.LogDebug(
            "Mapped {Count} roles for user {User}",
            rolesToAdd.Count,
            identity.FindFirst("preferred_username")?.Value ?? "unknown");
    }

    private static string GetFriendlyErrorMessage(JwtBearerChallengeContext context)
    {
        if (context.AuthenticateFailure is null)
            return "Authentication required. Please provide a valid access token.";

        var errorMessage = context.AuthenticateFailure.Message;

        return errorMessage switch
        {
            var msg when msg.Contains("expired") =>
                "Your session has expired. Please login again.",

            var msg when msg.Contains("audience") =>
                "Invalid token audience. Token is not intended for this API.",

            var msg when msg.Contains("issuer") =>
                "Invalid token issuer. Token was not issued by a trusted authority.",

            var msg when msg.Contains("signature") =>
                "Invalid token signature. Token may have been tampered with.",

            _ => "Invalid or expired access token. Please login again."
        };
    }
}