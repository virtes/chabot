using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace Chabot.Telegram.Commands;

[PublicAPI]
[AttributeUsage(AttributeTargets.Parameter)]
public class FromQueryParameterAttribute : Attribute
{
    public object? DefaultValue { get; set; }

    public string? Key { get; set; }
}