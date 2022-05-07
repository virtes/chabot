using System.Collections.Concurrent;
using Chabot.State.Configuration;
using Chabot.State.Exceptions;
using Microsoft.Extensions.Options;

namespace Chabot.State.Implementation;

public class StateTypeMapping : IStateTypeMapping
{
    private readonly StateTypeMappingOptions _options;
    private readonly ConcurrentDictionary<string, Type> _typesByName = new ();

    public StateTypeMapping(
        IOptions<StateTypeMappingOptions> optionsAccessor)
    {
        _options = optionsAccessor.Value;
    }
    
    public string GetStateTypeKey(Type stateType)
    {
        return stateType.FullName!;
    }

    public Type GetStateType(string stateTypeKey)
    {
        if (_typesByName.TryGetValue(stateTypeKey, out var type))
            return type;

        foreach (var assembly in _options.AssembliesToScan)
        {
            type = assembly.GetTypes().FirstOrDefault(t => t.FullName == stateTypeKey);
            if (type is not null)
                break;
        }

        if (type is null)
            throw new StateTypeNotFoundException(stateTypeKey);

        _typesByName.TryAdd(stateTypeKey, type);

        return type;
    }
}