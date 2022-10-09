using Chabot.Telegram.Configuration;
using Microsoft.Extensions.Options;
using Telegram.Bot;

namespace Chabot.Telegram.Implementation;

public class TelegramBotClientProvider : ITelegramBotClientProvider
{
    private readonly TelegramBotClient _telegramBotClient;
    
    public TelegramBotClientProvider(
        IOptions<TelegramBotOptions> optionsAccessor)
    {
        var options = optionsAccessor.Value;
        _telegramBotClient = new TelegramBotClient(options.Token);
    }
    
    public ITelegramBotClient GetClient()
    {
        return _telegramBotClient;
    }
}