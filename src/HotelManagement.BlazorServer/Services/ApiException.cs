namespace HotelManagement.BlazorServer.Services;


public class ApiException : Exception
{
    public ApiException(string message) : base(message) { }
    public ApiException(string message, Exception innerException) : base(message, innerException) { }
}

public class ApiUnauthorizedException : ApiException
{
    public ApiUnauthorizedException(string message) : base(message) { }
}

public class ApiForbiddenException : ApiException
{
    public ApiForbiddenException(string message) : base(message) { }
}