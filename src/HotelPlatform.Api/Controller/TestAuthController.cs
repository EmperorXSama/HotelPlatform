using System.Security.Claims;
using HotelPlatform.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelPlatform.Api.Controller;


[ApiController]
[Route("api/[controller]")]
public class TestAuthController : ControllerBase
{ 
    private readonly ICurrentUser _currentUser;

    public TestAuthController(ICurrentUser currentUser)
    {
        _currentUser = currentUser;
    }

    [HttpGet("public")]
    [AllowAnonymous]
    public IActionResult Public()
    {
        return Ok(new { message = "Public endpoint - no auth required" });
    }

    [HttpGet("authenticated")]
    [Authorize]
    public IActionResult Authenticated()
    {
        return Ok(new
        {
            message = "You are authenticated",
            userId = _currentUser.Id,
            email = _currentUser.Email,
            userName = _currentUser.UserName,
            roles = _currentUser.Roles
        });
    }

    [HttpGet("my-info")]
    [Authorize]
    public IActionResult MyInfo()
    {
        return Ok(new
        {
            id = _currentUser.Id,
            email = _currentUser.Email,
            userName = _currentUser.UserName,
            isAuthenticated = _currentUser.IsAuthenticated,
            roles = _currentUser.Roles,
            isSuperAdmin = _currentUser.IsInRole("super-admin"),
            isHotelOwner = _currentUser.HasAnyRole("hotel-owner", "super-admin")
        });
    }

    [HttpGet("admin-only")]
    [Authorize(Policy = "SuperAdmin")]
    public IActionResult AdminOnly()
    {
        return Ok(new 
        { 
            message = $"Welcome Super Admin: {_currentUser.UserName}",
            yourRoles = _currentUser.Roles
        });
    }
    
    [HttpGet("debug-claims")]
    [Authorize]
    public IActionResult DebugClaims()
    {
        // Don't serialize Claim objects directly - they have circular references
        var claims = User.Claims.Select(c => new 
        { 
            type = c.Type, 
            value = c.Value 
        }).ToList();
    
        var roleClaims = User.Claims
            .Where(c => c.Type == ClaimTypes.Role || c.Type == "roles" || c.Type.Contains("role"))
            .Select(c => new { type = c.Type, value = c.Value })
            .ToList();
    
        return Ok(new
        {
            claims,
            roleClaims,
            isInRoleSuperAdmin = User.IsInRole("super-admin"),
            distinctClaimTypes = User.Claims.Select(c => c.Type).Distinct().ToList()
        });
    }
}