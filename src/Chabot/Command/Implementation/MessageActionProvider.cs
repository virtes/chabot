using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace Chabot.Command.Implementation;

public class MessageActionProvider<TMessage, TUser>
    : IMessageActionProvider<TMessage, TUser>
{
    private readonly ILogger<MessageActionProvider<TMessage, TUser>> _logger;
    private readonly ICommandMessageActionBuilder<TMessage, TUser> _commandMessageActionBuilder;
    private readonly ICommandDescriptorSelector _commandDescriptorSelector;

    private readonly ConcurrentDictionary<CommandMessageActionKey,
        IMessageAction<TMessage, TUser>> _messageActions = new();

    public MessageActionProvider(
        ILogger<MessageActionProvider<TMessage, TUser>> logger,
        ICommandMessageActionBuilder<TMessage, TUser> commandMessageActionBuilder,
        ICommandDescriptorSelector commandDescriptorSelector)
    {
        _logger = logger;
        _commandMessageActionBuilder = commandMessageActionBuilder;
        _commandDescriptorSelector = commandDescriptorSelector;
    }
    
    public IMessageAction<TMessage, TUser>? GetMessageAction(
        ActionSelectionMetadata actionSelectionMetadata, Type stateType)
    {
        var commandDescriptor = _commandDescriptorSelector.GetCommandDescriptor(
            commandText: actionSelectionMetadata.CommandText,
            stateType: stateType);
        if (commandDescriptor is null)
        {
            _logger.LogWarning("Could not get command descriptor for {@SelectionMetadata}, {StateType}",
                actionSelectionMetadata, stateType.FullName);
            return null;
        }

        return GetCommandMessageAction(commandDescriptor);
    }

    private IMessageAction<TMessage, TUser> GetCommandMessageAction(
        CommandDescriptor commandDescriptor)
    {
        var key = new CommandMessageActionKey(
            type: commandDescriptor.Type, 
            method: commandDescriptor.Method);

        return _messageActions.GetOrAdd(key, 
            k =>
            {
                var action = _commandMessageActionBuilder.BuildInvokeCommand(
                    k.Type, k.Method);
                return new InvokeCommandMessageAction<TMessage, TUser>(k.Type, action);
            });
    }
    
    private readonly struct CommandMessageActionKey
    {
        public readonly Type Type;

        public readonly MethodInfo Method;

        public CommandMessageActionKey(Type type, MethodInfo method)
        {
            Type = type;
            Method = method;
        }
    }
}