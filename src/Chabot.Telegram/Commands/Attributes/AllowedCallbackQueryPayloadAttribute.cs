using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace Chabot.Telegram;

[PublicAPI]
[AttributeUsage(AttributeTargets.Method)]
public class AllowedCallbackQueryPayloadAttribute : Attribute
{
    public string[] AllowedPayloads { get; }

    public bool UseQueryParameters { get; set; } = false;

    public AllowedCallbackQueryPayloadAttribute(params string[] allowedPayloads)
    {
        AllowedPayloads = allowedPayloads;
    }
}