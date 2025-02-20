using System.Reflection;

namespace Chabot.Commands;

internal class FromServicesParameterValueResolverFactory<TUpdate>
    : ICommandParameterValueResolverFactory<TUpdate>
{
    public bool TryCreate(ParameterInfo parameterInfo, out ICommandParameterValueResolver<TUpdate> resolver)
    {
        var fromServicesAttribute = parameterInfo.GetCustomAttribute(typeof(FromServicesAttribute), true);
        if (fromServicesAttribute is null)
        {
            resolver = null!;
            return false;
        }

        resolver = new FromServicesParameterValueResolver<TUpdate>();
        return true;
    }
}