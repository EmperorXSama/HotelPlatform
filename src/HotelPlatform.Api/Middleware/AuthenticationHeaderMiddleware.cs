using System.Net;
using System.Text.Json;

namespace HotelPlatform.Api.Middleware;

public class AuthenticationHeaderMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AuthenticationHeaderMiddleware> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public AuthenticationHeaderMiddleware(
        RequestDelegate next,
        ILogger<AuthenticationHeaderMiddleware> logger)
    {
        _next = next;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        var allowAnonymous = endpoint?.Metadata.GetMetadata<Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute>();
        
        if (allowAnonymous is not null)
        {
            await _next(context);
            return;
        }
        
        var authHeader = context.Request.Headers.Authorization.FirstOrDefault();
        
        if (string.IsNullOrEmpty(authHeader))
        {
            _logger.LogDebug(
                "No Authorization header for {Method} {Path}",
                context.Request.Method,
                context.Request.Path);
        }
        else if (!authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning(
                "Invalid Authorization header format for {Method} {Path}",
                context.Request.Method,
                context.Request.Path);
            
            await WriteErrorResponseAsync(
                context,
                HttpStatusCode.Unauthorized,
                "InvalidAuthorizationHeader",
                "Authorization header must use Bearer scheme");
            return;
        }

        await _next(context);
    }

    private async Task WriteErrorResponseAsync(
        HttpContext context,
        HttpStatusCode statusCode,
        string errorCode,
        string message)
    {
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";

        var errorResponse = new
        {
            error = errorCode,
            message = message,
            timestamp = DateTime.UtcNow
        };

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(errorResponse, _jsonOptions));
    }
}