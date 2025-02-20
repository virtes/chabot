namespace Chabot.Commands;

internal interface ICommandActionExecutor<TUpdate>
{
    Task ExecuteCommandAction(CommandMetadata commandMetadata, UpdateContext<TUpdate> updateContext);
}