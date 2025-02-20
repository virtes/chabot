using Microsoft.Extensions.Logging;

namespace Chabot.Commands;

internal class CommandInvokerMiddleware<TUpdate> : IMiddleware<TUpdate>
{
    private readonly ICommandsProvider _commandsProvider;
    private readonly ICommandResolver<TUpdate> _commandResolver;
    private readonly ICommandActionExecutor<TUpdate> _commandActionExecutor;
    private readonly ILogger<CommandInvokerMiddleware<TUpdate>> _logger;

    public CommandInvokerMiddleware(ICommandsProvider commandsProvider,
        ICommandResolver<TUpdate> commandResolver,
        ICommandActionExecutor<TUpdate> commandActionExecutor,
        ILogger<CommandInvokerMiddleware<TUpdate>> logger)
    {
        _commandsProvider = commandsProvider;
        _commandResolver = commandResolver;
        _commandActionExecutor = commandActionExecutor;
        _logger = logger;
    }

    public async Task Invoke(UpdateContext<TUpdate> context, HandleUpdate<TUpdate> next)
    {
        var commandsMetadata = _commandsProvider.GetCommandsMetadata();

        var command = await _commandResolver.ResolveCommand(context.UpdateMetadata, commandsMetadata, context.Update);
        _logger.LogTrace("Command {CommandId} resolved", command.Id);

        await _commandActionExecutor.ExecuteCommandAction(command, context);
        _logger.LogDebug("Command {CommandId} executed", command.Id);

        await next(context);
    }
}