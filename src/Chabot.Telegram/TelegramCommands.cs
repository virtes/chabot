using Chabot.Commands;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Chabot.Telegram;

public class TelegramCommands : CommandsBase<Update>
{
    protected ITelegramBotClient BotClient
        => Context.ServiceProvider.GetRequiredService<ITelegramBotClientProvider>().GetClient();

    protected long ChatId
    {
        get
        {
            return Context.Update.Type switch
            {
                UpdateType.Message => Context.Update.Message!.Chat.Id,
                UpdateType.CallbackQuery => Context.Update.CallbackQuery!.Message!.Chat.Id,
                _ => throw new InvalidOperationException("Could not resolve chat id")
            };
        }
    }
}