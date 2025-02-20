using JetBrains.Annotations;
using Chabot.Telegram.Commands;

// ReSharper disable once CheckNamespace
namespace Chabot.Telegram;

[PublicAPI]
[AttributeUsage(AttributeTargets.Method)]
public class AllowedInlineQueryAttribute : UpdatePropertyRestrictionAttribute
{
    public AllowedInlineQueryAttribute(params string[] allowedValues)
        : base(TelegramUpdateProperties.InlineQuery, allowedValues)
    {
    }
}