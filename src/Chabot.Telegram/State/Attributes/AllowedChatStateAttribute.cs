using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace Chabot.Telegram;

[PublicAPI]
[AttributeUsage(AttributeTargets.Method)]
public class AllowedChatStateAttribute : AllowedStateAttribute
{
    public AllowedChatStateAttribute(params Type[] stateTypes)
        : base(TelegramStateTargetTypes.Chat, stateTypes)
    {
    }
}

[PublicAPI]
[AttributeUsage(AttributeTargets.Method)]
public class AllowedChatStateAttribute<TState> : AllowedChatStateAttribute
{
    public AllowedChatStateAttribute()
        : base(typeof(TState))
    {
    }
}