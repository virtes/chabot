using Microsoft.Extensions.Options;

namespace Chabot.Commands;

internal class CommandsProvider<TUpdate> : ICommandsProvider
{
    private readonly IOptions<CommandOptions> _options;
    private readonly CommandsCache _commandsCache;
    private readonly ICommandMetadataParser _commandMetadataParser;

    public CommandsProvider(IOptions<CommandOptions> options,
        CommandsCache commandsCache,
        ICommandMetadataParser commandMetadataParser)
    {
        _options = options;
        _commandsCache = commandsCache;
        _commandMetadataParser = commandMetadataParser;
    }

    public CommandMetadata[] GetCommandsMetadata()
    {
        // ReSharper disable once InconsistentlySynchronizedField
        if (_commandsCache.CommandsMetadata is not null)
            // ReSharper disable once InconsistentlySynchronizedField
            return _commandsCache.CommandsMetadata;

        lock (_commandsCache)
        {
            if (_commandsCache.CommandsMetadata is not null)
                return _commandsCache.CommandsMetadata;

            var commands = ScanCommands().ToArray();
            _commandsCache.CommandsMetadata = commands;
            return commands;
        }
    }

    private IReadOnlyList<CommandMetadata> ScanCommands()
    {
        var result = new List<CommandMetadata>();

        foreach (var assembly in _options.Value.AssembliesToScanCommands)
        {
            var commandBaseTypes = assembly.GetTypes()
                .Where(IsCommandsBaseType);

            foreach (var commandBaseType in commandBaseTypes)
            {
                result.AddRange(_commandMetadataParser.ParseCommands(commandBaseType));
            }
        }

        var commandsById = new Dictionary<string, CommandMetadata>(result.Count);
        foreach (var commandMetadata in result)
        {
            if (!commandsById.TryAdd(commandMetadata.Id, commandMetadata))
                throw new InvalidOperationException("Duplicated command");
        }

        return result;
    }

    private static bool IsCommandsBaseType(Type type)
    {
        var commandsBaseType = typeof(CommandsBase<TUpdate>);

        if (!type.IsClass)
            return false;

        if (type.IsAbstract)
            return false;

        var currentType = type;
        while (currentType != null)
        {
            if (currentType.IsGenericType)
            {
                if (currentType == commandsBaseType)
                    return true;
            }

            currentType = currentType.BaseType;
        }

        return false;
    }

    internal class CommandsCache
    {
        public CommandMetadata[]? CommandsMetadata { get; set; }
    }
}