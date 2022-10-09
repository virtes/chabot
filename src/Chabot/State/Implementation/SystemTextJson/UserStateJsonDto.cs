using System.Text.Json.Serialization;

namespace Chabot.State.Implementation.SystemTextJson;

public class UserStateJsonDto
{
    [JsonPropertyName("stateTypeKey")]
    public string StateTypeKey { get; set; } = default!;

    [JsonPropertyName("serializedState")]
    public string SerializedState { get; set; } = default!;

    [JsonPropertyName("createdAtUtc")]
    public DateTime CreatedAtUtc { get; set; }

    [JsonPropertyName("metadata")]
    public IReadOnlyDictionary<string, string?> Metadata { get; set; } = default!;
}