using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;

// ReSharper disable once CheckNamespace
namespace Chabot.Telegram;

public class TelegramBotClientOptions
{
    [Required(AllowEmptyStrings = false)]
    public string Token { get; set; } = default!;

    [ValidateObjectMembers]
    public ProxyOptions? Proxy { get; set; }

    public class ProxyOptions
    {
        [Required]
        public string Address { get; set; } = default!;

        [Required]
        public string Username { get; set; } = default!;

        [Required]
        public string Password { get; set; } = default!;
    }
}