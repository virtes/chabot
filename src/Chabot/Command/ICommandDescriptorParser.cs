namespace Chabot.Command;

public interface ICommandDescriptorParser
{
    IReadOnlyList<CommandDescriptor> ParseCommandDescriptors(Type commandGroupType);
}