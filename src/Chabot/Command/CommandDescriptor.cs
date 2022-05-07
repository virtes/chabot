using System.Reflection;

namespace Chabot.Command;

public class CommandDescriptor
{
    public Type Type { get; init; } = default!;

    public MethodInfo Method { get; init; } = default!;

    public bool AllowedWithAnyCommandText { get; init; } = false;

    public string[] CommandTexts { get; init; } = Array.Empty<string>();

    public bool AllowedInAnyState { get; init; } = false;

    public Type[] StateTypes { get; init; } = Array.Empty<Type>();

    public override string ToString() 
        => $"{AllowedWithAnyCommandText}, {AllowedInAnyState} " +
           $"([{string.Join(", ", CommandTexts)}], [{string.Join(", ", StateTypes.Select(st => st.Name))}])";
}