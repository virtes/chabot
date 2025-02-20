using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace Chabot;

[PublicAPI]
[AttributeUsage(AttributeTargets.Parameter)]
public abstract class FromStateAttribute : Attribute
{
    public string StateTargetType { get; }

    protected FromStateAttribute(string stateTargetType)
    {
        StateTargetType = stateTargetType;
    }
}