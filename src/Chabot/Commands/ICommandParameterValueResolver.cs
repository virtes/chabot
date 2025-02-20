using System.Reflection;

namespace Chabot.Commands;

public interface ICommandParameterValueResolver<TUpdate>
{
    ValueTask<object?> ResolveParameterValue(ParameterInfo parameterInfo, UpdateContext<TUpdate> updateContext);
}