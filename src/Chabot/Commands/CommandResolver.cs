using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Chabot.Commands;

internal class CommandResolver<TUpdate> : ICommandResolver<TUpdate>
{
    private readonly IServiceProvider _serviceProvider;
    private readonly RestrictionHandlersCache _restrictionHandlersCache;
    private readonly ILogger<CommandResolver<TUpdate>> _logger;

    public CommandResolver(IServiceProvider serviceProvider,
        RestrictionHandlersCache restrictionHandlersCache,
        ILogger<CommandResolver<TUpdate>> logger)
    {
        _serviceProvider = serviceProvider;
        _restrictionHandlersCache = restrictionHandlersCache;
        _logger = logger;
    }

    public async Task<CommandMetadata> ResolveCommand(UpdateMetadata updateMetadata,
        CommandMetadata[] commandsMetadata, TUpdate update)
    {
        var allowedCommands = new List<CommandMetadata>();

        foreach (var commandMetadata in commandsMetadata)
        {
            if (await IsAllowed(update, updateMetadata, commandMetadata))
            {
                allowedCommands.Add(commandMetadata);
            }
        }

        if (allowedCommands.Count == 1)
            return allowedCommands[0];

        if (allowedCommands.Count == 0)
            throw new InvalidOperationException("No suitable command found");

        var maxOrderCommand = allowedCommands.MaxBy(c => c.Order)!;
        if (allowedCommands.Any(c => c.Order == maxOrderCommand.Order
                                     && c.Id != maxOrderCommand.Id))
        {
            // ReSharper disable once CoVariantArrayConversion
            _logger.LogWarning("Ambiguous command configuration ({@CommandIds})",
                allowedCommands.Where(c => c.Order == maxOrderCommand.Order).Select(c => c.Id).ToArray());
            throw new InvalidOperationException("Ambiguous command configuration");
        }

        return maxOrderCommand;
    }

    private async Task<bool> IsAllowed(TUpdate update, UpdateMetadata updateMetadata, CommandMetadata commandMetadata)
    {
        foreach (var restriction in commandMetadata.Restrictions)
        {
            var restrictionType = restriction.GetType();

            if (!_restrictionHandlersCache.IsAllowedByRestrictionType.TryGetValue(restrictionType, out var func))
            {
                var handlerOpenType = typeof(ICommandRestrictionHandler<,>);

                var handlerClosedType = handlerOpenType.MakeGenericType(typeof(TUpdate), restrictionType);
                var isAllowedMethod = handlerClosedType.GetMethod("IsAllowed") ??
                                      throw new InvalidOperationException("Method not found");

                func = async (sp, u, um, r) =>
                {
                    var handler = sp.GetRequiredService(handlerClosedType);

                    var resultTask = (ValueTask<bool>) isAllowedMethod.Invoke(handler, new object?[] {u, um, r})!;
                    return await resultTask;
                };
                _restrictionHandlersCache.IsAllowedByRestrictionType[restrictionType] = func;
            }

            if (!await func(_serviceProvider, update, updateMetadata, restriction))
                return false;
        }

        return true;
    }

    internal delegate ValueTask<bool> IsAllowedFunc(
        IServiceProvider serviceProvider,
        TUpdate update,
        UpdateMetadata updateMetadata,
        object restriction);

    internal class RestrictionHandlersCache
    {
        public ConcurrentDictionary<Type, IsAllowedFunc> IsAllowedByRestrictionType { get; } = new();
    }
}