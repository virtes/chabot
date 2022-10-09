using System.Reflection;

namespace Chabot.State.Configuration;

public class StateOptions
{
    public HashSet<Assembly> AssembliesToScanStateTypes { get; } = new ();
}