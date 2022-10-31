using System.Reflection;
using Chabot.Command;
using Chabot.Message;

namespace Chabot.State.Implementation;

public class StateParameterValueResolver<TMessage, TUser>
    : ICommandParameterValueResolver<TMessage, TUser>
{
    public object? ResolveParameterValue(ParameterInfo parameterInfo,
        MessageContext<TMessage, TUser> messageContext)
    {
        var state = messageContext.UserState.State;
        if (!state.GetType().IsAssignableTo(parameterInfo.ParameterType))
            return null;

        return state;
    }
}