using System.Reflection;

namespace Chabot.Commands;

internal class UpdatePropertiesRestrictionsFactory : IRestrictionsFactory
{
    public IReadOnlyList<object> CreateRestrictions(Type type, MethodInfo methodInfo)
    {
        var result = new List<object>();

        var attributes = methodInfo.GetCustomAttributes<UpdatePropertyRestrictionAttribute>(true);
        foreach (var attribute in attributes.Reverse())
        {
            result.Add(new UpdatePropertiesRestriction
            {
                Key = attribute.Key,
                AllowedValues = attribute.AllowedValues
            });
        }

        return result;
    }
}