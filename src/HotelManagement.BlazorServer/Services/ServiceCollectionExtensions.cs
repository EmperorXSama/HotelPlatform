// Services/ServiceCollectionExtensions.cs

using System.Net;
using System.Text.Json;
using HotelManagement.BlazorServer.Http;
using HotelManagement.BlazorServer.Services.Files;
using HotelManagement.BlazorServer.Services.Hotel;
using HotelManagement.BlazorServer.Services.ReferenceData;
using Microsoft.Extensions.Http.Resilience;
using Polly;

namespace HotelManagement.BlazorServer.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHotelApiServices(
        this IServiceCollection services,
        string baseAddress)
    {
        // Configure JSON options shared across all API clients
        services.Configure<JsonSerializerOptions>(options =>
        {
            options.PropertyNameCaseInsensitive = true;
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });

        // Register the base API client with resilience policies
        services.AddHttpClient<IApiClient, ApiClient>(client =>
            {
                client.BaseAddress = new Uri(baseAddress);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.Timeout = TimeSpan.FromSeconds(30);
            })
            .AddHttpMessageHandler<TokenForwardingHandler>()  
            .AddResilienceHandler("hotel-api", ConfigureResilience);

        // Register domain-specific services
        services.AddScoped<IHotelApiService, HotelApiService>();
        services.AddScoped<IFileApiService, FileApiService>();
        services.AddScoped<IReferenceDataService, ReferenceDataService>();

        return services;
    }  
    public static IServiceCollection AddExternalHttpClientRegistration(
        this IServiceCollection services,
        string baseAddress)
    {
        // Configure JSON options shared across all API clients
        services.Configure<JsonSerializerOptions>(options =>
        {
            options.PropertyNameCaseInsensitive = true;
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });

        // Register the base API client with resilience policies
        services.AddHttpClient<IApiClient, ApiClient>(client =>
            {
                client.BaseAddress = new Uri(baseAddress);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.Timeout = TimeSpan.FromSeconds(30);
            })
            .AddHttpMessageHandler<TokenForwardingHandler>()  
            .AddResilienceHandler("hotel-api", ConfigureResilience);

        // Register domain-specific services
        services.AddScoped<IHotelApiService, HotelApiService>();
        services.AddScoped<IFileApiService, FileApiService>();
        services.AddScoped<IReferenceDataService, ReferenceDataService>();

        return services;
    }

    private static void ConfigureResilience(ResiliencePipelineBuilder<HttpResponseMessage> builder)
    {
        // Retry policy with exponential backoff
        builder.AddRetry(new HttpRetryStrategyOptions
        {
            MaxRetryAttempts = 3,
            Delay = TimeSpan.FromSeconds(1),
            BackoffType = DelayBackoffType.Exponential,
            UseJitter = true, // Adds randomness to prevent thundering herd
            ShouldHandle = args => ValueTask.FromResult(ShouldHandleTransientError(args.Outcome))
        });

        // Circuit breaker
        builder.AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions
        {
            SamplingDuration = TimeSpan.FromSeconds(30),
            FailureRatio = 0.5, // Open circuit if 50% of requests fail
            MinimumThroughput = 5, // Minimum requests before circuit can open
            BreakDuration = TimeSpan.FromSeconds(30),
            ShouldHandle = args => ValueTask.FromResult(ShouldHandleTransientError(args.Outcome))
        });

        // Timeout per request attempt
        builder.AddTimeout(TimeSpan.FromSeconds(10));
    }

    private static bool ShouldHandleTransientError(Outcome<HttpResponseMessage> outcome)
    {
        // Handle exceptions
        if (outcome.Exception is not null)
        {
            return outcome.Exception is HttpRequestException or TimeoutException;
        }

        // Handle specific status codes
        if (outcome.Result is null)
        {
            return false;
        }

        return outcome.Result.StatusCode is
            HttpStatusCode.RequestTimeout or
            HttpStatusCode.TooManyRequests or
            HttpStatusCode.InternalServerError or
            HttpStatusCode.BadGateway or
            HttpStatusCode.ServiceUnavailable or
            HttpStatusCode.GatewayTimeout;
    }
}