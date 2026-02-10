// Models/ProblemDetails.cs

using System.Text.Json.Serialization;

namespace HotelManagement.BlazorServer.models;

public sealed class ProblemDetails
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("status")]
    public int? Status { get; set; }

    [JsonPropertyName("detail")]
    public string? Detail { get; set; }

    [JsonPropertyName("instance")]
    public string? Instance { get; set; }

    [JsonPropertyName("errors")]
    public Dictionary<string, string[]>? Errors { get; set; }
    
    [JsonExtensionData]
    public Dictionary<string, object>? Extensions { get; set; }
}