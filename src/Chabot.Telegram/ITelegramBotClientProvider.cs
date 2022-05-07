using Telegram.Bot;

namespace Chabot.Telegram;

public interface ITelegramBotClientProvider
{
    ITelegramBotClient GetClient();
}