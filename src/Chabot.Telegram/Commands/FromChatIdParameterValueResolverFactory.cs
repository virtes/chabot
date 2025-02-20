using System.Reflection;
using Chabot.Commands;
using Chabot.Exceptions;
using Telegram.Bot.Types;

namespace Chabot.Telegram.Commands;

internal class FromChatIdParameterValueResolverFactory : ICommandParameterValueResolverFactory<Update>
{
    public bool TryCreate(ParameterInfo parameterInfo, out ICommandParameterValueResolver<Update> resolver)
    {
        var fromChatIdAttribute = parameterInfo.GetCustomAttribute(typeof(FromChatIdAttribute), true);
        if (fromChatIdAttribute is null)
        {
            resolver = null!;
            return false;
        }

        if (parameterInfo.ParameterType != typeof(long) && parameterInfo.ParameterType != typeof(long?))
        {
            throw new InvalidChabotConfigurationException(
                $"{nameof(FromChatIdAttribute)} can be used only with long or long? parameter");
        }

        resolver = parameterInfo.ParameterType == typeof(long)
            ? new FromChatIdParameterValueResolver()
            : new FromChatIdNullableParameterValueResolver();
        return true;
    }
}