using Telegram.Bot;

namespace Chabot.Telegram;

internal class TelegramBotClientProvider : ITelegramBotClientProvider
{
    private readonly ITelegramBotClient _telegramBotClient;

    public TelegramBotClientProvider(ITelegramBotClient telegramBotClient)
    {
        _telegramBotClient = telegramBotClient;
    }

    public ITelegramBotClient GetClient()
    {
        return _telegramBotClient;
    }
}