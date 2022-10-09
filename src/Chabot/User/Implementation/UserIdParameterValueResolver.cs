using System.Reflection;
using Chabot.Command;
using Chabot.Message;

namespace Chabot.User.Implementation;

public class UserIdParameterValueResolver<TMessage, TUser, TUserId>
    : ICommandParameterValueResolver<TMessage, TUser, TUserId>
    where TUser : IUser<TUserId>
    where TMessage : IMessage
{
    public object? ResolveParameterValue(ParameterInfo parameterInfo,
        MessageContext<TMessage, TUser, TUserId> messageContext)
    {
        return messageContext.User.Id;
    }
}