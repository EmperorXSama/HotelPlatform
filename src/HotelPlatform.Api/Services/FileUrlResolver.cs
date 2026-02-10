using HotelPlatform.Application.Common.Interfaces.Storage;
using HotelPlatform.Domain.Common.StronglyTypedIds;
using HotelPlatform.Domain.Files;
using HotelPlatform.Infrastructure.Storage;

namespace HotelPlatform.Api.Services;

public class FileUrlResolver : IFileUrlResolver
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public FileUrlResolver(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string GetAccessUrl(StoredFileId fileId)
    {
        var request = _httpContextAccessor.HttpContext?.Request;
        return $"{request?.Scheme}://{request?.Host}/api/files/{fileId.Value}";
    }
}