using System.Text.Json.Serialization;
using Chabot.User;

namespace Chabot.Telegram;

public class TgUser : IUser<long>
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("username")]
    public string? Username { get; set; }

    [JsonPropertyName("isBot")]
    public bool IsBot { get; set; }

    [JsonPropertyName("languageCode")]
    public string? LanguageCode { get; set; }
}