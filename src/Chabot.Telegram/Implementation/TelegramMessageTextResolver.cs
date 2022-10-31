using Chabot.Message;
using TelegramMessage = Telegram.Bot.Types.Message;

namespace Chabot.Telegram.Implementation;

public class TelegramMessageTextResolver : IMessageTextResolver<TelegramMessage>
{
    public string? GetMessageText(TelegramMessage message)
    {
        return message.Text;
    }
}