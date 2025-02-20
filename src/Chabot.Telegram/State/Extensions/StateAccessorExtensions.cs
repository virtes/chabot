using Chabot.State;
using Chabot.Telegram.State;
using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace Chabot.Telegram;

public static class StateAccessorExtensions
{
    [PublicAPI]
    public static async ValueTask SetChatState(this IStateAccessor stateAccessor,
        long chatId, object? state)
    {
        await stateAccessor.SetState(new ChatStateTarget(chatId), state);
    }

    [PublicAPI]
    public static async ValueTask SetChatMessageState(this IStateAccessor stateAccessor,
        long chatId, long messageId, object? state)
    {
        await stateAccessor.SetState(new ChatMessageStateTarget(chatId, messageId), state);
    }
}