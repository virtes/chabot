using System.Reflection;
using Chabot.Command;
using Chabot.Message;

namespace Chabot.User.Implementation;

public class UserIdParameterValueResolverFactory<TMessage, TUser, TUserId>
    : ICommandParameterValueResolverFactory<TMessage, TUser, TUserId>
    where TUser : IUser<TUserId>
    where TMessage : IMessage
{
    public ICommandParameterValueResolver<TMessage, TUser, TUserId>?
        CreateValueResolver(ParameterInfo parameterInfo)
    {
        if (parameterInfo.GetCustomAttribute<FromUserIdAttribute>() == null)
            return null;

        if (parameterInfo.ParameterType != typeof(TUserId))
            return null;

        return new UserIdParameterValueResolver<TMessage, TUser, TUserId>();
    }
}