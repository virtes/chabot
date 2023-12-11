using System.Reflection;
using Chabot.Command.Exceptions;
using Chabot.State;

namespace Chabot.Command.Implementation;

public class CommandDescriptorParser : ICommandDescriptorParser
{
    private readonly IServiceProvider _serviceProvider;

    public CommandDescriptorParser(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IReadOnlyList<CommandDescriptor> ParseCommandDescriptors(Type commandGroupType)
    {
        var result = new List<CommandDescriptor>();

        foreach (var methodInfo in commandGroupType.GetMethods(BindingFlags.Public | BindingFlags.Instance))
        {
            var commandAttribute = methodInfo.GetCustomAttribute<CommandAttributeBase>(true);
            if (commandAttribute is null)
                continue;

            var allowedWithAnyCommandText = commandAttribute.IsAllowedWithAnyCommandText(methodInfo);
            var commandTexts = commandAttribute.GetCommandTexts(methodInfo, _serviceProvider);

            if (!allowedWithAnyCommandText && commandTexts.Length == 0)
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

            var allowedStateAttributes = methodInfo.GetCustomAttributes<AllowedStateAttribute>().ToArray();
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
                AllowedWithAnyCommandText = allowedWithAnyCommandText,
                CommandTexts = commandTexts,
                AllowedInAnyState = allowedInAnyState,
                StateTypes = stateTypes.ToArray()
            };

            result.Add(commandDescriptor);
        }

        return result;
    }
}