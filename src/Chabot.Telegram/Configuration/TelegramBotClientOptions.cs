using System.ComponentModel.DataAnnotations;

namespace Chabot.Telegram.Configuration;

public class TelegramBotOptions
{
    [Required(AllowEmptyStrings = false)]
    public string Token { get; set; } = default!;
}