// Application/Common/Responses/ApiError.cs

namespace HotelPlatform.Application.Common.Responsse;

/// <summary>
/// Standardized error representation for API responses
/// </summary>
public sealed record ApiError
{
    public string Code { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string? Field { get; init; }
    public ErrorCategory Category { get; init; }

    public static ApiError FromErrorOr(Error error) => new()
    {
        Code = error.Code,
        Description = error.Description,
        Field = ExtractFieldName(error.Code),
        Category = MapErrorType(error.Type)
    };

    private static string? ExtractFieldName(string code)
    {
        // Extract field name from codes like "Validation.Email"
        if (code.StartsWith("Validation."))
        {
            return code["Validation.".Length..];
        }
        return null;
    }

    private static ErrorCategory MapErrorType(ErrorType type) => type switch
    {
        ErrorType.Validation => ErrorCategory.Validation,
        ErrorType.NotFound => ErrorCategory.NotFound,
        ErrorType.Conflict => ErrorCategory.Conflict,
        ErrorType.Unauthorized => ErrorCategory.Unauthorized,
        ErrorType.Forbidden => ErrorCategory.Forbidden,
        _ => ErrorCategory.General
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