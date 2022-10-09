namespace Chabot.Command;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class AllowedStateAttribute : Attribute
{
    public AllowedStateAttribute(Type stateType)
    {
        StateType = stateType;
    }

    public Type StateType { get; }
}