// Application/Common/Responses/ApiResponse.cs

namespace HotelPlatform.Application.Common.Responsse;

/// <summary>
/// Standard API response envelope for all endpoints
/// </summary>
public sealed class ApiResponse<T>
{
    public bool IsSuccess { get; init; }
    public T? Data { get; init; }
    public List<ApiError> Errors { get; init; } = [];
    public string? Message { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;

    // Factory methods for clean creation
    public static ApiResponse<T> Success(T data, string? message = null) => new()
    {
        IsSuccess = true,
        Data = data,
        Message = message
    };

    public static ApiResponse<T> Failure(List<ApiError> errors, string? message = null) => new()
    {
        IsSuccess = false,
        Errors = errors,
        Message = message
    };

    public static ApiResponse<T> Failure(ApiError error, string? message = null) => 
        Failure([error], message);
}

/// <summary>
/// Non-generic version for endpoints that don't return data
/// </summary>
public sealed class ApiResponse
{
    public bool IsSuccess { get; init; }
    public List<ApiError> Errors { get; init; } = [];
    public string? Message { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;

    public static ApiResponse Success(string? message = null) => new()
    {
        IsSuccess = true,
        Message = message
    };

    public static ApiResponse Failure(List<ApiError> errors, string? message = null) => new()
    {
        IsSuccess = false,
        Errors = errors,
        Message = message
    };

    public static ApiResponse Failure(ApiError error, string? message = null) => 
        Failure([error], message);
}