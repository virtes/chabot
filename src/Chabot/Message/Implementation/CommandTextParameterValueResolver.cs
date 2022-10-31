using System.Reflection;
using Chabot.Command;

namespace Chabot.Message.Implementation;

public class CommandTextParameterValueResolver<TMessage, TUser>
    : ICommandParameterValueResolver<TMessage, TUser>
{
    private readonly IMessageTextResolver<TMessage> _messageTextResolver;

    public CommandTextParameterValueResolver(
        IMessageTextResolver<TMessage> messageTextResolver)
    {
        _messageTextResolver = messageTextResolver;
    }

    public object? ResolveParameterValue(ParameterInfo parameterInfo,
        MessageContext<TMessage, TUser> messageContext)
    {
        return _messageTextResolver.GetMessageText(messageContext.Message);
    }
}