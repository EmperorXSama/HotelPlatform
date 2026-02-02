using System.Security.Claims;
using System.Text.Json;
using HotelPlatform.Application.Common.Interfaces;
using HotelPlatform.Application.Common.Interfaces.Repositories;
using HotelPlatform.Domain.Users;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;

namespace HotelPlatform.Api.Authentication;

public class JwtEventsHandler : JwtBearerEvents
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<JwtEventsHandler> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public JwtEventsHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        ILogger<JwtEventsHandler> logger)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public override async Task TokenValidated(TokenValidatedContext context)
    {
        if (context.Principal?.Identity is not ClaimsIdentity identity)
            return;

        // 1. Map Keycloak roles to standard claims (existing logic)
        MapKeycloakRolesToClaims(identity);

        // 2. JIT Provision: ensure user exists in our database
        await ProvisionUserAsync(identity);
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

    // ─── JIT User Provisioning ────────────────────────────────────────────

    private async Task ProvisionUserAsync(ClaimsIdentity identity)
    {
        var identityId = identity.FindFirst(ClaimTypes.NameIdentifier)?.Value
                         ?? identity.FindFirst("sub")?.Value;

        if (string.IsNullOrEmpty(identityId))
        {
            _logger.LogWarning("Token validated but no subject claim found");
            return;
        }

        try
        {
            // Check if user already exists (fast path — most requests hit this)
            var existingUser = await _userRepository.GetByIdentityIdAsync(identityId);
            if (existingUser is not null)
            {
                // Optionally sync profile changes from Keycloak
                SyncProfileIfChanged(existingUser, identity);
                return;
            }

            // First time seeing this user — create them
            var email = identity.FindFirst(ClaimTypes.Email)?.Value
                        ?? identity.FindFirst("email")?.Value
                        ?? string.Empty;

            var displayName = identity.FindFirst("preferred_username")?.Value
                              ?? identity.FindFirst("name")?.Value
                              ?? email;

            var user = User.Create(identityId, email, displayName);

            await _userRepository.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation(
                "Provisioned new user {UserId} for identity {IdentityId} ({Email})",
                user.Id.Value,
                identityId,
                email);
        }
        catch (DbUpdateException ex) when (IsDuplicateKeyException(ex))
        {
            // Race condition: two concurrent requests for the same new user.
            // The first one won, this one can safely ignore — user exists now.
            _logger.LogDebug(
                "User for identity {IdentityId} was created by a concurrent request",
                identityId);
        }
        catch (Exception ex)
        {
            // Don't fail the entire request if provisioning fails.
            // The user will be provisioned on the next request.
            _logger.LogError(
                ex,
                "Failed to provision user for identity {IdentityId}. Will retry on next request.",
                identityId);
        }
    }

    private void SyncProfileIfChanged(User user, ClaimsIdentity identity)
    {
        var email = identity.FindFirst(ClaimTypes.Email)?.Value
                    ?? identity.FindFirst("email")?.Value;

        var displayName = identity.FindFirst("preferred_username")?.Value
                          ?? identity.FindFirst("name")?.Value;

        if (email is null || displayName is null)
            return;

        // Only update if something actually changed
        if (string.Equals(user.Email, email, StringComparison.OrdinalIgnoreCase)
            && string.Equals(user.DisplayName, displayName, StringComparison.Ordinal))
            return;

        user.UpdateProfile(displayName, email, DateTime.UtcNow);

        // SaveChangesAsync is not called here — it will be saved
        // by the UnitOfWorkBehavior if the request is a command,
        // or we can explicitly save:
        _unitOfWork.SaveChangesAsync().GetAwaiter().GetResult();

        _logger.LogInformation(
            "Synced profile changes for user {IdentityId}", user.IdentityId);
    }

    private static bool IsDuplicateKeyException(DbUpdateException ex)
    {
        // PostgreSQL unique violation error code is 23505
        return ex.InnerException?.Message.Contains("23505") == true
               || ex.InnerException?.Message.Contains("duplicate key") == true;
    }

    // ─── Keycloak Role Mapping ────────────────────────────────────────────

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
                rolesToAdd.Add(claim.Value);
            }
        }

        // 3. Add all roles as standard ClaimTypes.Role claims
        foreach (var role in rolesToAdd)
        {
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