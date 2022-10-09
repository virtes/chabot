using System.Collections.Concurrent;
using System.Reflection;
using Chabot.Message;
using Chabot.User;
using Microsoft.Extensions.Logging;

namespace Chabot.Command.Implementation;

public class MessageActionProvider<TMessage, TUser, TUserId>
    : IMessageActionProvider<TMessage, TUser, TUserId> 
    where TMessage : IMessage
    where TUser : IUser<TUserId> 
{
    private readonly ILogger<MessageActionProvider<TMessage, TUser, TUserId>> _logger;
    private readonly ICommandMessageActionBuilder<TMessage, TUser, TUserId> _commandMessageActionBuilder;
    private readonly ICommandDescriptorSelector _commandDescriptorSelector;

    private readonly ConcurrentDictionary<CommandMessageActionKey,
        IMessageAction<TMessage, TUser, TUserId>> _messageActions = new();

    public MessageActionProvider(
        ILogger<MessageActionProvider<TMessage, TUser, TUserId>> logger,
        ICommandMessageActionBuilder<TMessage, TUser, TUserId> commandMessageActionBuilder,
        ICommandDescriptorSelector commandDescriptorSelector)
    {
        _logger = logger;
        _commandMessageActionBuilder = commandMessageActionBuilder;
        _commandDescriptorSelector = commandDescriptorSelector;
    }
    
    public IMessageAction<TMessage, TUser, TUserId>? GetMessageAction(
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

    private IMessageAction<TMessage, TUser, TUserId> GetCommandMessageAction(
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
                return new InvokeCommandMessageAction<TMessage, TUser, TUserId>(k.Type, action);
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