// Models/ApiResponse/ApiResponse.cs

namespace HotelManagement.BlazorServer.models.Response;

/// <summary>
/// Standard API response wrapper - mirrors the API's response structure
/// </summary>
public sealed class ApiResponse<T>
{
    public bool IsSuccess { get; init; }
    public T? Data { get; init; }
    public List<ApiError> Errors { get; init; } = [];
    public string? Message { get; init; }
    public DateTime Timestamp { get; init; }
}

/// <summary>
/// Non-generic version for endpoints without return data
/// </summary>
public sealed class ApiResponse
{
    public bool IsSuccess { get; init; }
    public List<ApiError> Errors { get; init; } = [];
    public string? Message { get; init; }
    public DateTime Timestamp { get; init; }
}

public sealed record ApiError
{
    public string Code { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string? Field { get; init; }
    public ErrorCategory Category { get; init; }

    /// <summary>
    /// Converts to ErrorOr Error type
    /// </summary>
    public Error ToError() => Category switch
    {
        ErrorCategory.Validation => Error.Validation(Code, Description),
        ErrorCategory.NotFound => Error.NotFound(Code, Description),
        ErrorCategory.Conflict => Error.Conflict(Code, Description),
        ErrorCategory.Unauthorized => Error.Unauthorized(Code, Description),
        ErrorCategory.Forbidden => Error.Forbidden(Code, Description),
        _ => Error.Failure(Code, Description)
    };
}

public enum ErrorCategory
{
    General,
    Validation,
    NotFound,
    Conflict,
    Unauthorized,
    Forbidden,
    Network,
    Server
}