using System.Reflection;
using Chabot.Command;
using Chabot.Message;
using Chabot.User;

namespace Chabot.State.Implementation;

public class StateParameterValueResolverFactory<TMessage, TUser, TUserId>
    : ICommandParameterValueResolverFactory<TMessage, TUser, TUserId>
    where TMessage : IMessage
    where TUser : IUser<TUserId>
{
    public ICommandParameterValueResolver<TMessage, TUser, TUserId>?
        CreateValueResolver(ParameterInfo parameterInfo)
    {
        if (!parameterInfo.ParameterType.IsAssignableTo(typeof(IState)))
            return null;

        return new StateParameterValueResolver<TMessage, TUser, TUserId>();
    }
}