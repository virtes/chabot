using System.Reflection;
using Chabot.Message;
using Chabot.User;

namespace Chabot.Command;

public interface ICommandParameterValueResolver<TMessage, TUser, TUserId>
    where TUser : IUser<TUserId>
    where TMessage : IMessage
{
    object? ResolveParameterValue(ParameterInfo parameterInfo,
        MessageContext<TMessage, TUser, TUserId> messageContext);
}