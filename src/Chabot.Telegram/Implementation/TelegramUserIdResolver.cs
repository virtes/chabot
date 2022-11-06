using Chabot.User;
using TelegramUpdate = Telegram.Bot.Types.Update;
using TelegramUser = Telegram.Bot.Types.User;

namespace Chabot.Telegram.Implementation;

public class TelegramUserIdResolver : IUserIdResolver<TelegramUpdate, TelegramUser>
{
    public object? GetUserId(TelegramUpdate message, TelegramUser user)
    {
        return user.Id;
    }
}