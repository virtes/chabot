using Chabot.State;
using Chabot.Telegram.State;
using JetBrains.Annotations;
using Telegram.Bot.Types;

// ReSharper disable once CheckNamespace
namespace Chabot.Telegram;

public static class UpdateContextExtensions
{
    [PublicAPI]
    public static async ValueTask SetCurrentChatState(this UpdateContext<Update> updateContext, object? state)
    {
        var telegramUpdate = updateContext.Update;
        var chatId = telegramUpdate.Message?.Chat.Id
                     ?? telegramUpdate.CallbackQuery?.Message?.Chat.Id
                     ?? throw new Exception("Could not determine chat id");

        await updateContext.SetState(new ChatStateTarget(chatId), state);
    }

    [PublicAPI]
    public static async ValueTask SetCurrentChatMessageState(this UpdateContext<Update> updateContext, object? state)
    {
        var telegramUpdate = updateContext.Update;
        var chatId = telegramUpdate.Message?.Chat.Id
                     ?? telegramUpdate.CallbackQuery?.Message?.Chat.Id
                     ?? throw new Exception("Could not determine chat id");

        var messageId = telegramUpdate.Message?.MessageId
                     ?? telegramUpdate.CallbackQuery?.Message?.MessageId
                     ?? throw new Exception("Could not determine message id");

        await updateContext.SetState(new ChatMessageStateTarget(chatId, messageId), state);
    }

    [PublicAPI]
    public static async ValueTask SetChatMessageState(this UpdateContext<Update> updateContext,
        long chatId, int messageId, object? state)
    {
        await updateContext.SetState(new ChatMessageStateTarget(chatId, messageId), state);
    }

    [PublicAPI]
    public static async ValueTask SetChatMessageState(this UpdateContext<Update> updateContext,
        Message message, object? state)
    {
        await updateContext.SetChatMessageState(message.Chat.Id, message.MessageId, state);
    }
}