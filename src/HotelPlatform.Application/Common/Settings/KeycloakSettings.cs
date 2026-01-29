namespace HotelPlatform.Application.Common.Settings;

public class KeycloakSettings
{
    public const string SectionName = "Keycloak";
    public string? Authority { get; set; }
    public string? Audience { get; set; }
}