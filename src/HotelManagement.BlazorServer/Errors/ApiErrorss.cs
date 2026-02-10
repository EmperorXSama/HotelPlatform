// Errors/ApiErrors.cs

namespace HotelManagement.BlazorServer.Errors;

public static class ApiErrors
{
    public static class Network
    {
        public static Error ConnectionFailed => Error.Failure(
            code: "Network.ConnectionFailed",
            description: "Unable to connect to the server. Please check your internet connection.");

        public static Error Timeout => Error.Failure(
            code: "Network.Timeout",
            description: "The request timed out. Please try again.");

        public static Error Cancelled => Error.Failure(
            code: "Network.Cancelled",
            description: "The request was cancelled.");
    }

    public static class Http
    {
        public static Error ServerError => Error.Failure(
            code: "Http.ServerError",
            description: "An unexpected server error occurred. Please try again later.");

        public static Error ServiceUnavailable => Error.Failure(
            code: "Http.ServiceUnavailable",
            description: "The service is temporarily unavailable. Please try again later.");

        public static Error Unauthorized => Error.Unauthorized(
            code: "Http.Unauthorized",
            description: "You are not authorized to perform this action. Please log in.");

        public static Error Forbidden => Error.Forbidden(
            code: "Http.Forbidden",
            description: "You do not have permission to access this resource.");

        public static Error NotFound(string resource) => Error.NotFound(
            code: "Http.NotFound",
            description: $"The requested {resource} was not found.");

        public static Error Conflict(string detail) => Error.Conflict(
            code: "Http.Conflict",
            description: detail);

        public static Error BadRequest(string detail) => Error.Validation(
            code: "Http.BadRequest",
            description: detail);
    }

    public static class Serialization
    {
        public static Error DeserializationFailed(string typeName) => Error.Failure(
            code: "Serialization.DeserializationFailed",
            description: $"Failed to deserialize the response to {typeName}.");
    }
}