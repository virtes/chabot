using System.Reflection;
using Chabot.Exceptions;

namespace Chabot.Commands;

internal class FromMessageTextParameterValueResolverFactory<TUpdate>
    : ICommandParameterValueResolverFactory<TUpdate>
{
    public bool TryCreate(ParameterInfo parameterInfo, out ICommandParameterValueResolver<TUpdate> resolver)
    {
        var fromMessageText = (FromMessageTextAttribute?)parameterInfo.GetCustomAttribute(typeof(FromMessageTextAttribute), true);
        if (fromMessageText is null)
        {
            resolver = null!;
            return false;
        }

        if (parameterInfo.ParameterType != typeof(string))
        {
            throw new InvalidChabotConfigurationException(
                $"{nameof(FromMessageTextAttribute)} can be used only with string parameter");
        }

        resolver = new FromMessageTextParameterValueResolver<TUpdate>();
        return true;
    }
}