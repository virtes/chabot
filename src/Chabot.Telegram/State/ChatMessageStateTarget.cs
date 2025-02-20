using Chabot.State;

namespace Chabot.Telegram.State;

public class ChatMessageStateTarget : IStateTarget
{
    public ChatMessageStateTarget(long chatId, long messageId)
    {
        Key = $"telegram:chat-message:{chatId}:{messageId}";
    }

    public string Key { get; }

    public string TargetType => TelegramStateTargetTypes.ChatMessage;
}