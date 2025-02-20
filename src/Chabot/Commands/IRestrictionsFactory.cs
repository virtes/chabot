using System.Reflection;

namespace Chabot.Commands;

public interface IRestrictionsFactory
{
    IReadOnlyList<object> CreateRestrictions(Type type, MethodInfo methodInfo);
}