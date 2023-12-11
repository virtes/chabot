using Chabot.Command.Configuration;
using Microsoft.Extensions.Options;

namespace Chabot.Command.Implementation;

public class CommandDescriptorsProvider : ICommandDescriptorsProvider
{
    private readonly ICommandDescriptorParser _commandDescriptorParser;
    private readonly CommandsOptions _options;

    public CommandDescriptorsProvider(IOptions<CommandsOptions> options,
        ICommandDescriptorParser commandDescriptorParser)
    {
        _commandDescriptorParser = commandDescriptorParser;
        _options = options.Value;
    }
    
    public IReadOnlyList<CommandDescriptor> GetCommandDescriptors()
    {
        var result = new List<CommandDescriptor>();

        foreach (var assembly in _options.AssembliesToScan)
        {
            var commandGroupTypes = assembly.GetTypes()
                .Where(IsCommandGroupType);

            foreach (var commandGroupType in commandGroupTypes)
            {
                result.AddRange(_commandDescriptorParser.ParseCommandDescriptors(commandGroupType));
            }
        }

        return result;
    }

    private static bool IsCommandGroupType(Type type)
    {
        var commandGroupOpenType = typeof(CommandGroupBase<,>);

        if (!type.IsClass)
            return false;

        if (type.IsAbstract)
            return false;

        var currentType = type;
        while (currentType != null)
        {
            if (currentType.IsGenericType)
            {
                var genericTypeDefinition = currentType.GetGenericTypeDefinition();
                if (genericTypeDefinition == commandGroupOpenType)
                    return true;
            }

            currentType = currentType.BaseType;
        }

        return false;
    }
}