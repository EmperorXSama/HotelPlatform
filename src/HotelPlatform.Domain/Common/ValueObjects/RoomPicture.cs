// Domain/Hotels/ValueObjects/RoomPicture.cs

using HotelPlatform.Domain.Common.StronglyTypedIds;

namespace HotelPlatform.Domain.Common.ValueObjects;

public sealed record RoomPicture
{
    public StoredFileId StoredFileId { get; }
    public string? AltText { get; init; }
    public bool IsMain { get; init; }
    public int DisplayOrder { get; init; }
    private RoomPicture()
    { }
    private RoomPicture(
        StoredFileId storedFileId,
        string? altText,
        bool isMain,
        int displayOrder)
    {
        StoredFileId = storedFileId;
        AltText = altText;
        IsMain = isMain;
        DisplayOrder = displayOrder;
    }

    public static RoomPicture Create(
        StoredFileId storedFileId,
        string? altText = null,
        bool isMain = false,
        int displayOrder = 0)
    {
        return new RoomPicture(storedFileId, altText?.Trim(), isMain, displayOrder);
    }

    public RoomPicture AsMain() => this with { IsMain = true };
    
    public RoomPicture AsSecondary() => this with { IsMain = false };
    
    public RoomPicture WithDisplayOrder(int order) => this with { DisplayOrder = order };
    
    public RoomPicture WithAltText(string? altText) => this with { AltText = altText?.Trim() };
}