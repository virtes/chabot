using System.Reflection;

namespace Chabot.Command.Configuration;

public class CommandsOptions
{
    public HashSet<Assembly> AssembliesToScan { get; } = new();
}