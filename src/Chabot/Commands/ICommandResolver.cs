namespace Chabot.Commands;

internal interface ICommandResolver<in TUpdate>
{
    Task<CommandMetadata> ResolveCommand(UpdateMetadata updateMetadata,
        CommandMetadata[] commandsMetadata, TUpdate update);
}