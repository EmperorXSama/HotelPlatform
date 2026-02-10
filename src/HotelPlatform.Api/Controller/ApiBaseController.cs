// Api/Controllers/ApiBaseController.cs

using ErrorOr;
using HotelPlatform.Application.Common.Responsse;
using Microsoft.AspNetCore.Mvc;

namespace HotelPlatform.Api.Controller;

[ApiController]
[Route("api/[controller]")]
public abstract class ApiBaseController : ControllerBase
{
    /// <summary>
    /// Converts ErrorOr result to standardized ApiResponse with appropriate HTTP status
    /// </summary>
    protected IActionResult ToApiResponse<T>(ErrorOr<T> result, string? successMessage = null)
    {
        return result.Match(
            value => Ok(ApiResponse<T>.Success(value, successMessage)),
            errors => ToErrorResponse<T>(errors)
        );
    }

    /// <summary>
    /// Converts ErrorOr result for operations without return data
    /// </summary>
    protected IActionResult ToApiResponse(ErrorOr<Success> result, string? successMessage = null)
    {
        return result.Match(
            _ => Ok(ApiResponse.Success(successMessage)),
            errors => ToErrorResponse(errors)
        );
    }

    /// <summary>
    /// Returns Created response with location header
    /// </summary>
    protected IActionResult ToCreatedResponse<T>(
        ErrorOr<T> result, 
        string actionName,
        Func<T, object> routeValuesFactory,
        string? successMessage = null)
    {
        return result.Match(
            value => CreatedAtAction(
                actionName,
                routeValuesFactory(value),
                ApiResponse<T>.Success(value, successMessage)),
            errors => ToErrorResponse<T>(errors)
        );
    }

    /// <summary>
    /// Returns NoContent for successful delete operations
    /// </summary>
    protected IActionResult ToNoContentResponse(ErrorOr<Success> result)
    {
        return result.Match(
            _ => NoContent(),
            errors => ToErrorResponse(errors)
        );
    }

    private IActionResult ToErrorResponse<T>(List<Error> errors)
    {
        var apiErrors = errors.Select(ApiError.FromErrorOr).ToList();
        var response = ApiResponse<T>.Failure(apiErrors);
        
        return ToStatusCodeResult(errors, response);
    }

    private IActionResult ToErrorResponse(List<Error> errors)
    {
        var apiErrors = errors.Select(ApiError.FromErrorOr).ToList();
        var response = ApiResponse.Failure(apiErrors);
        
        return ToStatusCodeResult(errors, response);
    }

    private IActionResult ToStatusCodeResult<T>(List<Error> errors, T response)
    {
        var firstError = errors.First();

        return firstError.Type switch
        {
            ErrorType.Validation => BadRequest(response),
            ErrorType.NotFound => NotFound(response),
            ErrorType.Conflict => Conflict(response),
            ErrorType.Unauthorized => Unauthorized(response),
            ErrorType.Forbidden => StatusCode(StatusCodes.Status403Forbidden, response),
            _ => StatusCode(StatusCodes.Status500InternalServerError, response)
        };
    }
}