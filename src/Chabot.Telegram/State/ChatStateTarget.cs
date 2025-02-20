using Chabot.State;

namespace Chabot.Telegram.State;

public class ChatStateTarget : IStateTarget
{
    public ChatStateTarget(long chatId)
    {
        Key = $"telegram:chat:{chatId}";
    }

    public string Key { get; }

    public string TargetType => TelegramStateTargetTypes.Chat;
}