using System.Reflection;
using Chabot.Commands;

namespace Chabot.Telegram.Commands;

internal class AllowedUpdateTypeRestrictionsFactory : IRestrictionsFactory
{
    public IReadOnlyList<object> CreateRestrictions(Type type, MethodInfo methodInfo)
    {
        var methodAllowedUpdateType = methodInfo.GetCustomAttribute<AllowedUpdateTypeAttribute>();
        if (methodAllowedUpdateType is not null)
            return [ new AllowedUpdateTypeRestriction(methodAllowedUpdateType.AllowedUpdateTypes) ];

        var typeAllowedUpdateType = type.GetCustomAttribute<AllowedUpdateTypeAttribute>();
        if (typeAllowedUpdateType is not null)
            return [ new AllowedUpdateTypeRestriction(typeAllowedUpdateType.AllowedUpdateTypes) ];

        return Array.Empty<object>();
    }
}