using System.Text;

namespace HotelPlatform.Api.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private ILogger<RequestLoggingMiddleware> _logger;
    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        context.Request.EnableBuffering();

        using var reader = new StreamReader(
            context.Request.Body,
            Encoding.UTF8,
            detectEncodingFromByteOrderMarks: false,
            bufferSize: 1024,
            leaveOpen: true
        );
        
        var body = await reader.ReadToEndAsync();
        
        context.Request.Body.Position = 0;
        
        _logger.LogInformation("Request {Method} {Path}: {Body}", 
            context.Request.Method, 
            context.Request.Path, 
            body);

        await _next(context);
    }
}