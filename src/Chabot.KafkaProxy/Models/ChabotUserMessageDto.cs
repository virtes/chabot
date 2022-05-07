using System.Text.Json.Serialization;

namespace Chabot.KafkaProxy.Models;

public class ChabotUserMessageDto<TUser, TMessage>
{
    [JsonPropertyName("user")]
    public TUser User { get; set; } = default!;

    [JsonPropertyName("message")]
    public TMessage Message { get; set; } = default!;
}