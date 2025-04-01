using System.Reflection;
using Chabot.Commands;

namespace Chabot.Telegram.Commands;

internal class AllowedCallbackQueryPayloadRestrictionsFactory : IRestrictionsFactory
{
    public IReadOnlyList<object> CreateRestrictions(Type type, MethodInfo methodInfo)
    {
        var attribute = methodInfo.GetCustomAttribute<AllowedCallbackQueryPayloadAttribute>();
        if (attribute is null)
            return Array.Empty<object>();

        return [ new AllowedCallbackQueryPayloadRestriction(attribute.AllowedPayloads, attribute.UseQueryParameters) ];
    }
}