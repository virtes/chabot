using System.Reflection;

namespace Chabot.Commands;

public class CommandMetadata
{
    public required string Id { get; init; }

    public required Type Type { get; init; }

    public required MethodInfo MethodInfo { get; init; }

    public required IReadOnlyList<object> Restrictions { get; init; }

    public required int Order { get; init; }
}