using System.Reflection;
using Chabot.Command;

namespace Chabot.User.Implementation;

public class UserIdParameterValueResolverFactory<TMessage, TUser>
    : ICommandParameterValueResolverFactory<TMessage, TUser>
{
    private readonly IUserIdResolver<TMessage, TUser> _userIdResolver;

    public UserIdParameterValueResolverFactory(
        IUserIdResolver<TMessage, TUser> userIdResolver)
    {
        _userIdResolver = userIdResolver;
    }
    public ICommandParameterValueResolver<TMessage, TUser>?
        CreateValueResolver(ParameterInfo parameterInfo)
    {
        if (parameterInfo.GetCustomAttribute<FromUserIdAttribute>() == null)
            return null;

        return new UserIdParameterValueResolver<TMessage, TUser>(_userIdResolver);
    }
}