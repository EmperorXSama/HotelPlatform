namespace HotelManagement.BlazorServer.models.Response.RefereneceData;

public class SelectedAmenityModel
{
    public Guid AmenityId { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string? IconCode { get; set; }
    public string? Category { get; set; }

    public UpchargeType UpchargeType { get; set; } = UpchargeType.Flat;
    public decimal UpchargeAmount { get; set; }
    public string? Currency { get; set; } = "USD";
    
    public bool HasUpcharge => UpchargeAmount > 0;

    public string GetUpchargeDisplay()
    {
        if (!HasUpcharge) return "Free";

        return UpchargeType switch
        {
            UpchargeType.Flat => $"{GetCurrencySymbol()}{UpchargeAmount:N2}",
            UpchargeType.PercentagePerNight => $"{UpchargeAmount:P0}/night",
            _ => string.Empty
        };
    }

    private string GetCurrencySymbol() => Currency switch
    {
        "USD" => "$",
        "EUR" => "€",
        _ => "DH"
        
    };
}

public enum UpchargeType
{
    Flat = 0,
    PercentagePerNight = 1
}

public class AmenitySelectorConfig
{
    /// <summary>
    /// Type of amenities being selected (Hotel or Room)
    /// </summary>
    public AmenityContextType ContextType { get; set; } = AmenityContextType.Hotel;
    
    /// <summary>
    /// Whether to show upcharge editing capabilities
    /// </summary>
    public bool AllowUpchargeEditing { get; set; } = true;
    
    /// <summary>
    /// Available currencies for flat upcharges
    /// </summary>
    public List<string> AvailableCurrencies { get; set; } = ["USD", "EUR", "GBP"];
    
    /// <summary>
    /// Default currency for new upcharges
    /// </summary>
    public string DefaultCurrency { get; set; } = "USD";
    
    /// <summary>
    /// Maximum number of amenities that can be selected (0 = unlimited)
    /// </summary>
    public int MaxSelections { get; set; } = 0;
    
    /// <summary>
    /// Number of items to show before "Show more" button
    /// </summary>
    public int InitialVisibleCount { get; set; } = 6;
    
    /// <summary>
    /// Title displayed on the selector card
    /// </summary>
    public string Title { get; set; } = "Select Amenities";

    
}

public enum AmenityContextType
{
    Hotel,
    Room
}
