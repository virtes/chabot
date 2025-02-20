namespace Chabot.Commands;

internal interface ICommandMetadataParser
{
    IReadOnlyList<CommandMetadata> ParseCommands(Type type);
}