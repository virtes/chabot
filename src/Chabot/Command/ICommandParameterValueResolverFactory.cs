using System.Reflection;
using Chabot.Message;
using Chabot.User;

namespace Chabot.Command;

public interface ICommandParameterValueResolverFactory<TMessage, TUser, TUserId>
    where TMessage : IMessage
    where TUser : IUser<TUserId>
{
    ICommandParameterValueResolver<TMessage, TUser, TUserId>?
        CreateValueResolver(ParameterInfo parameterInfo);
}