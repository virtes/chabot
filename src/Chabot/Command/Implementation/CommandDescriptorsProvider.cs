using System.Reflection;
using Chabot.Command.Configuration;
using Chabot.Command.Exceptions;
using Chabot.State;
using Microsoft.Extensions.Options;

namespace Chabot.Command.Implementation;

public class CommandDescriptorsProvider : ICommandDescriptorsProvider
{
    private readonly CommandsOptions _options;

    public CommandDescriptorsProvider(IOptions<CommandsOptions> options)
    {
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
                result.AddRange(GetCommandGroupDescriptors(commandGroupType));
            }
        }

        return result;
    }

    private IEnumerable<CommandDescriptor> GetCommandGroupDescriptors(Type commandGroupType)
    {
        var result = new List<CommandDescriptor>();

        foreach (var methodInfo in commandGroupType.GetMethods(BindingFlags.Public | BindingFlags.Instance))
        {
            var commandAttribute = methodInfo.GetCustomAttribute<CommandAttribute>();
            if (commandAttribute is null)
                continue;

            var allowedStateAttributes = methodInfo.GetCustomAttributes<AllowedStateAttribute>().ToArray();

            if (!commandAttribute.AllowedWithAnyCommandText && commandAttribute.CommandTexts.Length == 0)
            {
                throw new InvalidCommandActionException(commandGroupType, methodInfo,
                    $"action is unreachable ({nameof(CommandAttribute.AllowedWithAnyCommandText)} " +
                    $"is false and no command texts are specified)");
            }

            var allowedInAnyState = methodInfo.GetCustomAttribute<AllowedInAnyStateAttribute>() != null;
            var stateTypes = new HashSet<Type>();

            foreach (var parameter in methodInfo.GetParameters())
            {
                var parameterType = parameter.ParameterType;

                if (!parameterType.IsAssignableTo(typeof(IState)))
                    continue;

                stateTypes.Add(parameterType);
            }

            foreach (var allowedStateAttribute in allowedStateAttributes)
            {
                if (!allowedStateAttribute.StateType.IsAssignableTo(typeof(IState)))
                {
                    throw new InvalidCommandActionException(commandGroupType, methodInfo,
                        $"action allowed state ({nameof(AllowedStateAttribute)}) " +
                        $"has invalid state type");
                }

                stateTypes.Add(allowedStateAttribute.StateType);
            }

            if (!allowedInAnyState && stateTypes.Count == 0)
            {
                throw new InvalidCommandActionException(commandGroupType, methodInfo,
                    $"action is unreachable (not allowed in any state and no {nameof(IState)} state arguments)");
            }

            var commandDescriptor = new CommandDescriptor
            {
                Type = commandGroupType,
                Method = methodInfo,
                AllowedWithAnyCommandText = commandAttribute.AllowedWithAnyCommandText,
                CommandTexts = commandAttribute.CommandTexts,
                AllowedInAnyState = allowedInAnyState,
                StateTypes = stateTypes.ToArray()
            };
            
            result.Add(commandDescriptor);
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

