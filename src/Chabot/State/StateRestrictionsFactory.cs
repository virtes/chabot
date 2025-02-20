using System.Reflection;
using Chabot.Commands;

namespace Chabot.State;

internal class StateRestrictionsFactory : IRestrictionsFactory
{
    public IReadOnlyList<object> CreateRestrictions(Type type, MethodInfo methodInfo)
    {
        var result = new List<object>();

        var attributes = methodInfo.GetCustomAttributes<AllowedStateAttribute>(true);
        foreach (var allowedStateAttribute in attributes)
        {
            result.Add(new StateRestriction
            {
                AllowedStateTypes = allowedStateAttribute.StateTypes,
                StateTargetType = allowedStateAttribute.StateTargetType,
                AllowEmptyState = allowedStateAttribute.AllowEmptyState
            });
        }

        return result;
    }
}