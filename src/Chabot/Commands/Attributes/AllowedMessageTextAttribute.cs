using JetBrains.Annotations;
using Chabot.Commands;

// ReSharper disable once CheckNamespace
namespace Chabot;

[PublicAPI]
[AttributeUsage(AttributeTargets.Method)]
public class AllowedMessageTextAttribute : UpdatePropertyRestrictionAttribute
{
    public AllowedMessageTextAttribute(params string[] allowedValues)
        : base(UpdateProperties.MessageText, allowedValues)
    {
    }
}