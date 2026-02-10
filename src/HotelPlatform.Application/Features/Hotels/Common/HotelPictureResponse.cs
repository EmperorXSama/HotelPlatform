namespace HotelPlatform.Application.Features.Hotels.Common;

public sealed record HotelPictureResponse(
    Guid StoredFileId,
    string Url,    
    bool IsMain,
    int SortOrder);