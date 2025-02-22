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

    protected int MessageId
    {
        get
        {
            return Context.Update.Type switch
            {
                UpdateType.Message => Context.Update.Message!.MessageId,
                UpdateType.CallbackQuery => Context.Update.CallbackQuery!.Message!.MessageId,
                _ => throw new InvalidOperationException("Could not resolve message id")
            };
        }
    }

    protected long UserId
    {
        get
        {
            return Context.Update.Type switch
            {
                UpdateType.Message => Context.Update.Message!.From!.Id,
                UpdateType.CallbackQuery => Context.Update.CallbackQuery!.From.Id,
                _ => throw new InvalidOperationException("Could not resolve user id")
            };
        }
    }

    protected string? CallbackQueryPayload
    {
        get
        {
            return Context.Update.Type switch
            {
                UpdateType.CallbackQuery => Context.Update.CallbackQuery!.Data,
                _ => throw new InvalidOperationException("Could not resolve callback query payload")
            };
        }
    }
}