using System.Reflection;
using Chabot.Command;
using Chabot.Message;
using Chabot.User;

namespace Chabot.State.Implementation;

public class StateParameterValueResolver<TMessage, TUser, TUserId>
    : ICommandParameterValueResolver<TMessage, TUser, TUserId>
    where TUser : IUser<TUserId>
    where TMessage : IMessage
{
    public object? ResolveParameterValue(ParameterInfo parameterInfo,
        MessageContext<TMessage, TUser, TUserId> messageContext)
    {
        var state = messageContext.UserState.State;
        if (!state.GetType().IsAssignableTo(parameterInfo.ParameterType))
            return null;

        return state;
    }
}