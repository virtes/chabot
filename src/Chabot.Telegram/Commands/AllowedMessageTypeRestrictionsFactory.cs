using System.Reflection;
using Chabot.Commands;

namespace Chabot.Telegram.Commands;

public class AllowedMessageTypeRestrictionsFactory : IRestrictionsFactory
{
    public IReadOnlyList<object> CreateRestrictions(Type type, MethodInfo methodInfo)
    {
        var attribute = methodInfo.GetCustomAttribute<AllowedMessageTypeAttribute>();
        if (attribute is null)
            return Array.Empty<object>();

        return new object[]
        {
            new AllowedMessageTypeRestriction(attribute.MessageTypes)
        };
    }
}