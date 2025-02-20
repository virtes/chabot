using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace Chabot;

[PublicAPI]
[AttributeUsage(AttributeTargets.Parameter)]
public class FromChatIdAttribute : Attribute
{
}