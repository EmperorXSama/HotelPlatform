namespace HotelPlatform.Api.Middleware;


public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseAuthenticationHeaderValidation(
        this IApplicationBuilder app)
    {
        return app.UseMiddleware<AuthenticationHeaderMiddleware>();
    }

    public static IApplicationBuilder UseCustomMiddlewares(this IApplicationBuilder app)
    {
        app.UseMiddleware<RequestLoggingMiddleware>();
        return app;
    }
}