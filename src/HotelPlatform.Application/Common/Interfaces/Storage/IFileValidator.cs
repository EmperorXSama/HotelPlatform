using Microsoft.AspNetCore.Http;

namespace HotelPlatform.Application.Common.Interfaces.Storage;

public interface IFileValidator
{
    ErrorOr<Success> Validate(IFormFile file);
    ErrorOr<Success> Validate(string fileName, string contentType, long sizeInBytes);
}