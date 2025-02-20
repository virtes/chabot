using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace Chabot.Telegram;

[PublicAPI]
[AttributeUsage(AttributeTargets.Method)]
public class AllowedChatMessageStateAttribute : AllowedStateAttribute
{
    public AllowedChatMessageStateAttribute(params Type[] allowedTypes)
        : base(TelegramStateTargetTypes.ChatMessage, allowedTypes)
    {
    }
}


[PublicAPI]
[AttributeUsage(AttributeTargets.Method)]
public class AllowedChatMessageStateAttribute<TState> : AllowedChatMessageStateAttribute
{
    public AllowedChatMessageStateAttribute()
        : base(typeof(TState))
    {
    }
}