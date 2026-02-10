namespace HotelManagement.BlazorServer.models.Response.Hotels;

public sealed record HotelPictureResponse(
    Guid StoredFileId,
    string Url,    
    bool IsMain,
    int DisplayOrder);