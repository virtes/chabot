using System.Reflection;
using Chabot.Command;
using Chabot.User;

namespace Chabot.Message.Implementation;

public class CommandTextParameterValueResolver<TMessage, TUser, TUserId>
    : ICommandParameterValueResolver<TMessage, TUser, TUserId>
    where TUser : IUser<TUserId>
    where TMessage : IMessage
{
    public object? ResolveParameterValue(ParameterInfo parameterInfo,
        MessageContext<TMessage, TUser, TUserId> messageContext)
    {
        return messageContext.Message.Text;
    }
}