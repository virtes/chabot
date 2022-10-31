using Chabot.Command;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using TelegramMessage = Telegram.Bot.Types.Message;
using TelegramUser = Telegram.Bot.Types.User;

namespace Chabot.Telegram;

public abstract class TelegramCommandGroup
    : CommandGroupBase<TelegramMessage, TelegramUser>
{
    protected long UserId => User.Id;

    protected long ChatId => Message.Chat.Id;

    protected int MessageId => Message.MessageId;

    protected string? MessageText => Message.Text ?? Message.Caption;

    protected ITelegramBotClient BotClient
    {
        get
        {
            var clientProvider = Context.Services.GetRequiredService<ITelegramBotClientProvider>();
            return clientProvider.GetClient();
        }
    }
}