using Chabot.State;
using Telegram.Bot.Types.Enums;
using TelegramUpdate = Telegram.Bot.Types.Update;
using TelegramUser = Telegram.Bot.Types.User;

namespace Chabot.Telegram.Implementation;

public class TelegramStateTargetFactory
    : IStateTargetFactory<TelegramUpdate, TelegramUser, TelegramStateTarget>
{
    public TelegramStateTarget GetStateTarget(TelegramUpdate message, TelegramUser user)
    {
        return message.Type switch
        {
            UpdateType.Message when message.Message?.From != null
                => new TelegramStateTarget(
                    userId: message.Message.From.Id,
                    chatId: message.Message.Chat.Id,
                    messageId: null),
            UpdateType.CallbackQuery when message.CallbackQuery?.Message?.From != null
                => new TelegramStateTarget(
                    message.CallbackQuery.From.Id,
                    message.CallbackQuery.Message.Chat.Id,
                    message.CallbackQuery.Message.MessageId),
            _ => throw new InvalidOperationException("Could not determine state target")
        };
    }
}