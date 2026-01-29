using System.Security.Claims;
using System.Text.Json;
using HotelPlatform.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace HotelPlatform.Infrastructure.Identity;

public class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private IReadOnlyList<string>? _roles;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public string? Id => User?.FindFirstValue(ClaimTypes.NameIdentifier) 
                         ?? User?.FindFirstValue("sub");

    public string? Email => User?.FindFirstValue(ClaimTypes.Email) 
                            ?? User?.FindFirstValue("email");

    public string? UserName => User?.FindFirstValue("preferred_username") 
                               ?? User?.Identity?.Name;

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

    public IReadOnlyList<string> Roles => _roles ??= GetRoles();

    public bool IsInRole(string role) => Roles.Contains(role, StringComparer.OrdinalIgnoreCase);

    public bool HasAnyRole(params string[] roles) => roles.Any(IsInRole);

    private IReadOnlyList<string> GetRoles()
    {
        if (User is null)
            return new List<string>().AsReadOnly();

        var roles = new List<string>();

        // Try to get roles from multiple sources
        var roleClaims = User.FindAll("roles")
            .Concat(User.FindAll(ClaimTypes.Role))
            .ToList();

        foreach (var claim in roleClaims)
        {
            // Check if the claim value is a JSON array
            if (claim.Value.StartsWith("["))
            {
                try
                {
                    var parsed = JsonSerializer.Deserialize<string[]>(claim.Value);
                    if (parsed != null)
                        roles.AddRange(parsed);
                }
                catch
                {
                    // Not valid JSON, add as-is
                    roles.Add(claim.Value);
                }
            }
            else
            {
                roles.Add(claim.Value);
            }
        }

        return roles.Distinct().ToList().AsReadOnly();
    }
}