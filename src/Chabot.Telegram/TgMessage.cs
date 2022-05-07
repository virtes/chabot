using System.Text.Json.Serialization;
using Chabot.Message;

namespace Chabot.Telegram;

public class TgMessage : IMessage
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("text")]
    public string? Text { get; set; }
}