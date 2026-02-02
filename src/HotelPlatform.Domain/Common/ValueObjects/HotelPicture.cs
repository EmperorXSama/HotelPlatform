using HotelPlatform.Domain.Common.StronglyTypedIds;

namespace HotelPlatform.Domain.Common.ValueObjects;
public sealed record HotelPicture
{
    public StoredFileId StoredFileId { get; init; }
    public string? AltText { get; init; }
    public bool IsMain { get; init; }
    public int DisplayOrder { get; init; }
    private HotelPicture()
    { }
    private HotelPicture(
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

    public static HotelPicture Create(
        StoredFileId storedFileId,
        string? altText = null,
        bool isMain = false,
        int displayOrder = 0)
    {
        return new HotelPicture(storedFileId, altText?.Trim(), isMain, displayOrder);
    }

    public HotelPicture AsMain() => this with { IsMain = true }; 
    
    public HotelPicture AsSecondary() => this with { IsMain = false };
    
    public HotelPicture WithDisplayOrder(int order) => this with { DisplayOrder = order };
    
    public HotelPicture WithAltText(string? altText) => this with { AltText = altText?.Trim() };
}