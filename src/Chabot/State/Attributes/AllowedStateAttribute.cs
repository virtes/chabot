using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace Chabot;

[PublicAPI]
[AttributeUsage(AttributeTargets.Method)]
public class AllowedStateAttribute : Attribute
{
    public string StateTargetType { get; }
    public Type[] StateTypes { get; }
    public bool AllowEmptyState { get; set; }

    public AllowedStateAttribute(string stateTargetType, Type[] stateTypes)
    {
        StateTargetType = stateTargetType;
        StateTypes = stateTypes;
    }
}