using System.Reflection;
using Chabot.Command;

namespace Chabot.State.Implementation;

public class StateParameterValueResolverFactory<TMessage, TUser>
    : ICommandParameterValueResolverFactory<TMessage, TUser>
{
    public ICommandParameterValueResolver<TMessage, TUser>?
        CreateValueResolver(ParameterInfo parameterInfo)
    {
        if (!parameterInfo.ParameterType.IsAssignableTo(typeof(IState)))
            return null;

        return new StateParameterValueResolver<TMessage, TUser>();
    }
}