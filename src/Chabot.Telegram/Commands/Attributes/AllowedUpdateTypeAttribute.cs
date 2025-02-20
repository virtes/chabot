using JetBrains.Annotations;
using Telegram.Bot.Types.Enums;

// ReSharper disable once CheckNamespace
namespace Chabot.Telegram;

[PublicAPI]
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class AllowedUpdateTypeAttribute : Attribute
{
    public UpdateType[] AllowedUpdateTypes { get; }

    public AllowedUpdateTypeAttribute(params UpdateType[] allowedUpdateTypes)
    {
        AllowedUpdateTypes = allowedUpdateTypes;
    }
}