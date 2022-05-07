namespace Chabot.Command;

public interface ICommandDescriptorSelector
{
    CommandDescriptor? GetCommandDescriptor(string? commandText, Type? stateType);
}