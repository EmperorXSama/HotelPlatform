// Infrastructure/Storage/FileValidator.cs
using ErrorOr;
using HotelPlatform.Application.Common.Errors;
using HotelPlatform.Application.Common.Interfaces.Storage;
using HotelPlatform.Application.Common.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace HotelPlatform.Infrastructure.Storage;

public class FileValidator : IFileValidator
{
    private readonly FileValidationSettings _settings;

    public FileValidator(IOptions<StorageSettings> options)
    {
        _settings = options.Value.Validation;
    }

    public ErrorOr<Success> Validate(IFormFile file)
    {
        return Validate(file.FileName, file.ContentType, file.Length);
    }

    public ErrorOr<Success> Validate(string fileName, string contentType, long sizeInBytes)
    {
        var errors = new List<Error>();

        // Validate size
        if (sizeInBytes > _settings.MaxFileSizeBytes)
        {
            errors.Add(StorageErrors.FileTooLarge(_settings.MaxFileSizeBytes));
        }

        // Validate content type
        if (!_settings.AllowedContentTypes.Contains(contentType, StringComparer.OrdinalIgnoreCase))
        {
            errors.Add(StorageErrors.InvalidContentType(contentType));
        }

        // Validate extension
        var extension = Path.GetExtension(fileName);
        if (!_settings.AllowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
        {
            errors.Add(StorageErrors.InvalidExtension(extension));
        }

        return errors.Count > 0 ? errors : Result.Success;
    }
}