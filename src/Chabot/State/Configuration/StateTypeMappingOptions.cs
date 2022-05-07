using System.Reflection;

namespace Chabot.State.Configuration;

public class StateTypeMappingOptions
{
    public List<Assembly> AssembliesToScan { get; } = new ();
}