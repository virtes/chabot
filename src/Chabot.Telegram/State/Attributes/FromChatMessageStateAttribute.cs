using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace Chabot.Telegram;

[PublicAPI]
[AttributeUsage(AttributeTargets.Parameter)]
public class FromChatMessageStateAttribute : FromStateAttribute
{
    public FromChatMessageStateAttribute() : base(TelegramStateTargetTypes.ChatMessage)
    {
    }
}