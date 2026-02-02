using HotelPlatform.Domain.Common.StronglyTypedIds;

namespace HotelPlatform.Domain.ReferenceData;

public class HotelAmenityDefinition
{
    public HotelAmenityDefinitionId Id { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Icon { get; private set; }
    public string? Category { get; private set; }
    public bool IsActive { get; private set; }
    public bool IsSystemDefined { get; private set; }
    
    private HotelAmenityDefinition() { }
    
    public static HotelAmenityDefinition Create(
        string code,
        string name,
        string? icon = null,
        string? category = null,
        bool isSystemDefined = false)
    {
        return new HotelAmenityDefinition
        {
            Id = HotelAmenityDefinitionId.New(),
            Code = code.ToUpperInvariant(),
            Name = name,
            Icon = icon,
            Category = category,
            IsActive = true,
            IsSystemDefined = isSystemDefined
        };
    }
    
    public static HotelAmenityDefinition CreateSystem(
        string code,
        string name,
        string? icon = null,
        string? category = null)
    {
        return Create(code, name, icon, category, isSystemDefined: true);
    }
    public void Update(string name, string? icon, string? category)
    {
        Name = name;
        Icon = icon;
        Category = category;
    }
    public void Activate() => IsActive = true;
    
    public void Deactivate() => IsActive = false;
}