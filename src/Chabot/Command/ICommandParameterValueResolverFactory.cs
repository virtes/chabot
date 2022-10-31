using System.Reflection;

namespace Chabot.Command;

public interface ICommandParameterValueResolverFactory<TMessage, TUser>
{
    ICommandParameterValueResolver<TMessage, TUser>? CreateValueResolver(
        ParameterInfo parameterInfo);
}