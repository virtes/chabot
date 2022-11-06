using Chabot.Command;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using TelegramUpdate = Telegram.Bot.Types.Update;
using TelegramUser = Telegram.Bot.Types.User;

namespace Chabot.Telegram;

public abstract class TelegramCommandGroup
    : CommandGroupBase<TelegramUpdate, TelegramUser>
{
    protected long UserId => User.Id;

    protected long ChatId
        => Message.Message?.Chat.Id
           ?? Message.CallbackQuery?.Message?.Chat.Id
           ?? throw new InvalidOperationException("Could not determine chat id");

    protected int MessageId
        => Message.Message?.MessageId
           ?? Message.CallbackQuery?.Message?.MessageId
           ?? throw new InvalidOperationException("Could not determine message id");

    protected string? MessageText
        => Message.Message?.Text
           ?? Message.Message?.Caption;

    protected ITelegramBotClient BotClient
    {
        get
        {
            var clientProvider = Context.Services.GetRequiredService<ITelegramBotClientProvider>();
            return clientProvider.GetClient();
        }
    }
}