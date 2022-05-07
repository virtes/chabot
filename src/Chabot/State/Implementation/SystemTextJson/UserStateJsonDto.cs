using System.Text.Json.Serialization;

namespace Chabot.State.Implementation.SystemTextJson;

public class UserStateJsonDto
{
    [JsonPropertyName("stateTypeKey")]
    public string? StateTypeKey { get; set; }

    [JsonPropertyName("serializedState")]
    public string? SerializedState { get; set; }

    [JsonPropertyName("createdAtUtc")]
    public DateTime CreatedAtUtc { get; set; }
    
    [JsonPropertyName("metadata")]
    public Dictionary<string, string?>? Metadata { get; set; }
}