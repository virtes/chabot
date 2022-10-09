using System.Reflection;
using Chabot.Command;
using Chabot.User;

namespace Chabot.Message.Implementation;

public class CommandTextParameterValueResolverFactory<TMessage, TUser, TUserId>
    : ICommandParameterValueResolverFactory<TMessage, TUser, TUserId>
    where TUser : IUser<TUserId>
    where TMessage : IMessage
{
    public ICommandParameterValueResolver<TMessage, TUser, TUserId>?
        CreateValueResolver(ParameterInfo parameterInfo)
    {
        if (parameterInfo.GetCustomAttribute<FromMessageTextAttribute>() is null)
            return null;

        if (parameterInfo.ParameterType != typeof(string))
            return null;

        return new CommandTextParameterValueResolver<TMessage, TUser, TUserId>();
    }
}