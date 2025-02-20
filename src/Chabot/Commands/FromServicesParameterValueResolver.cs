using System.Reflection;

namespace Chabot.Commands;

internal class FromServicesParameterValueResolver<TUpdate>
    : ICommandParameterValueResolver<TUpdate>
{
    public ValueTask<object?> ResolveParameterValue(
        ParameterInfo parameterInfo, UpdateContext<TUpdate> updateContext)
    {
        return ValueTask.FromResult(updateContext.ServiceProvider.GetService(parameterInfo.ParameterType));
    }
}