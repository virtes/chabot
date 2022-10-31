using System.Reflection;
using Chabot.Command;

namespace Chabot.Message.Implementation;

public class CommandTextParameterValueResolverFactory<TMessage, TUser>
    : ICommandParameterValueResolverFactory<TMessage, TUser>
{
    private readonly IMessageTextResolver<TMessage> _messageTextResolver;

    public CommandTextParameterValueResolverFactory(
        IMessageTextResolver<TMessage> messageTextResolver)
    {
        _messageTextResolver = messageTextResolver;
    }

    public ICommandParameterValueResolver<TMessage, TUser>?
        CreateValueResolver(ParameterInfo parameterInfo)
    {
        if (parameterInfo.GetCustomAttribute<FromMessageTextAttribute>() is null)
            return null;

        if (parameterInfo.ParameterType != typeof(string))
            return null;

        return new CommandTextParameterValueResolver<TMessage, TUser>(_messageTextResolver);
    }
}