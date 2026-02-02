using HotelPlatform.Domain.Common.StronglyTypedIds;

namespace HotelPlatform.Domain.ReferenceData;


public class RoomTypeDefinition
{
    public RoomTypeDefinitionId Id { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public int DefaultCapacity { get; private set; }
    public string? Icon { get; private set; }
    public bool IsActive { get; private set; }
    public bool IsSystemDefined { get; private set; }

    private RoomTypeDefinition() { }

    public static RoomTypeDefinition Create(
        string code,
        string name,
        int defaultCapacity,
        string? description = null,
        string? icon = null,
        bool isSystemDefined = false)
    {
        return new RoomTypeDefinition
        {
            Id = RoomTypeDefinitionId.New(),
            Code = code.ToUpperInvariant(),
            Name = name,
            Description = description,
            DefaultCapacity = Math.Max(1, defaultCapacity),
            Icon = icon,
            IsActive = true,
            IsSystemDefined = isSystemDefined
        };
    }

    public static RoomTypeDefinition CreateSystem(
        string code,
        string name,
        int defaultCapacity,
        string? description = null,
        string? icon = null)
    {
        return Create(code, name, defaultCapacity, description, icon, isSystemDefined: true);
    }

    public void Update(string name, string? description, int defaultCapacity, string? icon)
    {
        Name = name;
        Description = description;
        DefaultCapacity = Math.Max(1, defaultCapacity);
        Icon = icon;
    }

    public void Activate() => IsActive = true;
    
    public void Deactivate() => IsActive = false;
}