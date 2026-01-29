namespace HotelPlatform.Api.Middleware;


public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseAuthenticationHeaderValidation(
        this IApplicationBuilder app)
    {
        return app.UseMiddleware<AuthenticationHeaderMiddleware>();
    }
}