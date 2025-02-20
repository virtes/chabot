using System.Reflection;

namespace Chabot.Commands;

public class CommandOptions
{
    internal List<Assembly> AssembliesToScanCommands { get; } = new();
}