using Chabot.Message;
using TelegramUpdate = Telegram.Bot.Types.Update;

namespace Chabot.Telegram.Implementation;

public class TelegramMessageTextResolver : IMessageTextResolver<TelegramUpdate>
{
    public string? GetMessageText(TelegramUpdate message)
    {
        return message.Message?.Text ?? message.Message?.Caption;
    }
}