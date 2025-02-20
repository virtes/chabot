using System.ComponentModel.DataAnnotations;

// ReSharper disable once CheckNamespace
namespace Chabot.Telegram;

public class TelegramBotClientOptions
{
    [Required(AllowEmptyStrings = false)]
    public string? Token { get; set; } = default!;
}