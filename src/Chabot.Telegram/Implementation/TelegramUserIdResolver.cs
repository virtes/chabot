using Chabot.User;
using TelegramMessage = Telegram.Bot.Types.Message;
using TelegramUser = Telegram.Bot.Types.User;

namespace Chabot.Telegram.Implementation;

public class TelegramUserIdResolver : IUserIdResolver<TelegramMessage, TelegramUser>
{
    public object? GetUserId(TelegramMessage message, TelegramUser user)
    {
        return user.Id;
    }
}