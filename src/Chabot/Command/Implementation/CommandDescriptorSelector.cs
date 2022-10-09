namespace Chabot.Command.Implementation;

public class CommandDescriptorSelector : ICommandDescriptorSelector
{
    public CommandDescriptorSelector(
        ICommandDescriptorsProvider commandDescriptorsProvider)
    {
        _allCommandDescriptors = commandDescriptorsProvider.GetCommandDescriptors()
            .OrderByDescending(cd => cd.AllowedInAnyState && cd.AllowedWithAnyCommandText)
                .ThenBy(cd => cd.CommandTexts.Length)
                    .ThenBy(cd => cd.StateTypes.Length)
            .ToArray();
        _descriptorsByCommandText = BuildDescriptorsMapByCommandText(_allCommandDescriptors);
        _anyCommandTextDescriptorsByStateType = BuildAnyCommandTextDescriptorsMapByStateType(_allCommandDescriptors);
    }

    private static Dictionary<string, List<CommandDescriptor>> BuildDescriptorsMapByCommandText(
        CommandDescriptor[] commandDescriptors)
    {
        var result = new Dictionary<string, List<CommandDescriptor>>();
        
        foreach (var commandDescriptor in commandDescriptors
                     .OrderBy(cd => cd.AllowedWithAnyCommandText)
                        .ThenBy(cd => cd.CommandTexts.Length)
                            .ThenBy(cd => cd.AllowedInAnyState)
                                .ThenBy(cd => cd.StateTypes.Length))
        {
            foreach (var commandTextSource in commandDescriptor.CommandTexts)
            {
                var commandText = commandTextSource.ToLowerInvariant();

                if (!result.TryGetValue(commandText, out var commandTextDescriptors))
                {
                    commandTextDescriptors = new List<CommandDescriptor>();
                    result[commandText] = commandTextDescriptors;
                }
                
                commandTextDescriptors.Add(commandDescriptor);
            }
        }

        return result;
    }

    private static Dictionary<Type, CommandDescriptor> BuildAnyCommandTextDescriptorsMapByStateType(
        CommandDescriptor[] commandDescriptors)
    {
        var result = new Dictionary<Type, CommandDescriptor>();

        foreach (var commandDescriptor in commandDescriptors
                     .Where(cd => cd.AllowedWithAnyCommandText)
                     .OrderBy(cd => cd.AllowedInAnyState)
                        .ThenBy(cd => cd.StateTypes.Length)
                            .ThenBy(cd => cd.AllowedWithAnyCommandText)
                                .ThenBy(cd => cd.CommandTexts.Length))
        {
            foreach (var stateType in commandDescriptor.StateTypes)
            {
                if (result.ContainsKey(stateType))
                    continue;
                
                result[stateType] = commandDescriptor;
            }
        }

        return result;
    }

    // Descriptors sorted by CommandTexts.Length and NOT allowed with any command text
    private readonly Dictionary<string, List<CommandDescriptor>> _descriptorsByCommandText;
    // Descriptors sorted by StateTypes.Length and NOT allowed in any state type
    private readonly Dictionary<Type, CommandDescriptor> _anyCommandTextDescriptorsByStateType;
    // All descriptors sorted by AllowedWithAnyCommandText && AllowedInAnyState
    private readonly CommandDescriptor[] _allCommandDescriptors;

    public CommandDescriptor? GetCommandDescriptor(string? commandText, Type stateType)
    {
        commandText = commandText?.ToLowerInvariant();

        if (commandText is not null && _descriptorsByCommandText.TryGetValue(commandText, out var commandTextDescriptors))
        {
            foreach (var commandTextDescriptor in commandTextDescriptors)
            {
                foreach (var commandTextDescriptorStateType in commandTextDescriptor.StateTypes)
                {
                    if (stateType == commandTextDescriptorStateType)
                        return commandTextDescriptor;
                }
            }

            foreach (var commandTextDescriptor in commandTextDescriptors)
            {
                if (commandTextDescriptor.AllowedInAnyState)
                    return commandTextDescriptor;
            }
        }

        if (_anyCommandTextDescriptorsByStateType.TryGetValue(stateType, out var stateTypeDescriptor))
        {
            return stateTypeDescriptor;
        }

        foreach (var commandDescriptor in _allCommandDescriptors)
        {
            if (commandDescriptor.AllowedWithAnyCommandText && commandDescriptor.AllowedInAnyState)
                return commandDescriptor;
        }
        
        return null;
    }
}