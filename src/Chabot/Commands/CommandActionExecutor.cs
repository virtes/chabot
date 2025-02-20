using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;

namespace Chabot.Commands;

internal class CommandActionExecutor<TUpdate> : ICommandActionExecutor<TUpdate>
{
    private readonly CommandActionsCache _commandActionsCache;
    private readonly ICommandActionBuilder<TUpdate> _commandActionBuilder;

    public CommandActionExecutor(CommandActionsCache commandActionsCache,
        ICommandActionBuilder<TUpdate> commandActionBuilder)
    {
        _commandActionsCache = commandActionsCache;
        _commandActionBuilder = commandActionBuilder;
    }

    public async Task ExecuteCommandAction(CommandMetadata commandMetadata, UpdateContext<TUpdate> updateContext)
    {
        var commandAction = _commandActionsCache.CommandActionsByCommandMetadata
            .GetOrAdd(commandMetadata, cm => _commandActionBuilder.BuildCommandAction(cm.Type, cm.MethodInfo));

        var instance = (CommandsBase<TUpdate>) ActivatorUtilities
            .GetServiceOrCreateInstance(updateContext.ServiceProvider, commandMetadata.Type);
        instance.Context = updateContext;

        await commandAction(instance, updateContext);
    }

    internal class CommandActionsCache
    {
        public ConcurrentDictionary<CommandMetadata, CommandAction<TUpdate>> CommandActionsByCommandMetadata { get; } = new();
    }
}