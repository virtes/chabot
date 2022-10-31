using System.Reflection;
using Chabot.Message;

namespace Chabot.Command;

public interface ICommandParameterValueResolver<TMessage, TUser>
{
    object? ResolveParameterValue(ParameterInfo parameterInfo,
        MessageContext<TMessage, TUser> messageContext);
}