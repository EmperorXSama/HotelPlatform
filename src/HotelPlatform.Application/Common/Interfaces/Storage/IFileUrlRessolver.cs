namespace HotelPlatform.Application.Common.Interfaces.Storage;

public interface IFileUrlResolver
{
    string GetAccessUrl(StoredFileId storedFile);
}