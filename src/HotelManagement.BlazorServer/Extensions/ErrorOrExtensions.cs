
namespace HotelManagement.BlazorServer.Extensions;

public static class ErrorOrExtensions
{
    /// <summary>
    /// Extracts error descriptions as a list of strings for display
    /// </summary>
    public static List<string> ToErrorMessages(this List<Error> errors)
    {
        return errors.Select(e => e.Description).ToList();
    }

    /// <summary>
    /// Gets the first error or a default message
    /// </summary>
    public static string FirstErrorMessage(this List<Error> errors, string defaultMessage = "An error occurred")
    {
        return errors.FirstOrDefault().Description ?? defaultMessage;
    }

    /// <summary>
    /// Checks if any error matches a specific code
    /// </summary>
    public static bool HasErrorCode(this List<Error> errors, string code)
    {
        return errors.Any(e => e.Code == code);
    }

    /// <summary>
    /// Checks if unauthorized error is present
    /// </summary>
    public static bool IsUnauthorized(this List<Error> errors)
    {
        return errors.Any(e => e.Type == ErrorType.Unauthorized);
    }

    /// <summary>
    /// Groups validation errors by field
    /// </summary>
    public static Dictionary<string, List<string>> ToValidationDictionary(this List<Error> errors)
    {
        return errors
            .Where(e => e.Type == ErrorType.Validation)
            .GroupBy(e => e.Code.Replace("Validation.", ""))
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.Description).ToList()
            );
    }
}