using System.Reflection;

namespace Chabot.Commands;

public interface ICommandParameterValueResolverFactory<TUpdate>
{
    bool TryCreate(ParameterInfo parameterInfo, out ICommandParameterValueResolver<TUpdate> resolver);
}