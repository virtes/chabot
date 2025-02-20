namespace Chabot.Commands;

internal interface ICommandsProvider
{
    CommandMetadata[] GetCommandsMetadata();
}