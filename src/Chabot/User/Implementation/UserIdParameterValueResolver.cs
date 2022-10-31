using System.Reflection;
using Chabot.Command;
using Chabot.Message;

namespace Chabot.User.Implementation;

public class UserIdParameterValueResolver<TMessage, TUser>
    : ICommandParameterValueResolver<TMessage, TUser>
{
    private readonly IUserIdResolver<TMessage, TUser> _userIdResolver;

    public UserIdParameterValueResolver(
        IUserIdResolver<TMessage, TUser> userIdResolver)
    {
        _userIdResolver = userIdResolver;
    }

    public object? ResolveParameterValue(ParameterInfo parameterInfo,
        MessageContext<TMessage, TUser> messageContext)
    {
        return _userIdResolver.GetUserId(messageContext.Message, messageContext.User);
    }
}