using System.Reflection;
using Chabot.Commands;

namespace Chabot.State;

internal class StateCommandParameterValueResolverFactory<TUpdate> : ICommandParameterValueResolverFactory<TUpdate>
{
    public bool TryCreate(ParameterInfo parameterInfo, out ICommandParameterValueResolver<TUpdate> resolver)
    {
        var fromStateAttribute = (FromStateAttribute?)parameterInfo.GetCustomAttribute(typeof(FromStateAttribute), true);
        if (fromStateAttribute is null)
        {
            resolver = null!;
            return false;
        }

        resolver = new StateCommandParameterValueResolver<TUpdate>(fromStateAttribute.StateTargetType);
        return true;
    }
}