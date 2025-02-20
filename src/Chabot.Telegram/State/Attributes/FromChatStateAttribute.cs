using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace Chabot.Telegram;

[PublicAPI]
[AttributeUsage(AttributeTargets.Parameter)]
public class FromChatStateAttribute : FromStateAttribute
{
    public FromChatStateAttribute() : base(TelegramStateTargetTypes.Chat)
    {
    }
}