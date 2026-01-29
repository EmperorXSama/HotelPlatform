using System.ComponentModel.DataAnnotations;

namespace HotelPlatform.Application.Common.Settings;

public class DatabaseSettings
{
    public const string SectionName = "Database";
    
    [Required]
    public string ConnectionString { get; set; } = string.Empty;
    [Range(1, 10)]
    public int MaxRetryCount { get; set; } = 3;
    [Range(30, 60)]
    public int CommandTimeOut { get; set; } = 30;
}