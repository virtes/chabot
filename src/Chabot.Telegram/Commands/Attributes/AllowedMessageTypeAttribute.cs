using JetBrains.Annotations;
using Telegram.Bot.Types.Enums;

// ReSharper disable once CheckNamespace
namespace Chabot.Telegram.Commands;

[PublicAPI]
[AttributeUsage(AttributeTargets.Method)]
public class AllowedMessageTypeAttribute : Attribute
{
    public MessageType[] MessageTypes { get; }

    public AllowedMessageTypeAttribute(params MessageType[] messageTypes)
    {
        MessageTypes = messageTypes;
    }
}