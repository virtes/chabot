using System.Reflection;
using System.Text.RegularExpressions;

namespace Chabot.Commands;

internal class AllowedMessageTextRegexRestrictionsFactory : IRestrictionsFactory
{
    public IReadOnlyList<object> CreateRestrictions(Type type, MethodInfo methodInfo)
    {
        var attribute = methodInfo.GetCustomAttribute<AllowedMessageTextRegexAttribute>(true);
        if (attribute is null)
            return [];

        return new object[]
        {
            new AllowedMessageTextRegexRestriction
            {
                Regex = new Regex(attribute.Pattern, RegexOptions.Compiled)
            }
        };
    }
}