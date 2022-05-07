using System.ComponentModel.DataAnnotations;

namespace Chabot.Telegram.Configuration;

public class TelegramBotClientOptions
{
    [Required(AllowEmptyStrings = false)]
    public string Token { get; set; } = default!;
}