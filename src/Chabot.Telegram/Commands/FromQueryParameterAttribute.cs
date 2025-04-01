using JetBrains.Annotations;

namespace Chabot.Telegram.Commands;

[PublicAPI]
[AttributeUsage(AttributeTargets.Parameter)]
public class FromQueryParameterAttribute : Attribute
{
    public object? DefaultValue { get; set; }

    public string? Key { get; set; }
}