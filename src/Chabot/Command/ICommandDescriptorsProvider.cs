namespace Chabot.Command;

public interface ICommandDescriptorsProvider
{
    IReadOnlyList<CommandDescriptor> GetCommandDescriptors();
}