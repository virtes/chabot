using System.Reflection;

namespace Chabot.Commands;

internal class CommandMetadataParser : ICommandMetadataParser
{
    private readonly IRestrictionsFactory[] _restrictionsFactories;

    public CommandMetadataParser(IEnumerable<IRestrictionsFactory> restrictionsFactories)
    {
        _restrictionsFactories = restrictionsFactories.ToArray();
    }

    public IReadOnlyList<CommandMetadata> ParseCommands(Type type)
    {
        var result = new List<CommandMetadata>();

        foreach (var methodInfo in type.GetMethods(BindingFlags.Public | BindingFlags.Instance))
        {
            if (methodInfo.DeclaringType != type)
                continue;

            var commandMetadata = new CommandMetadata
            {
                Id = GetCommandId(methodInfo),
                Type = type,
                MethodInfo = methodInfo,
                Restrictions = GetCommandRestrictions(type, methodInfo),
                Order = GetCommandOrder(methodInfo)
            };
            result.Add(commandMetadata);
        }

        return result;
    }

    private static int GetCommandOrder(MethodInfo methodInfo)
    {
        var orderAttribute = methodInfo.GetCustomAttribute<OrderAttribute>();
        return orderAttribute?.Order ?? 0;
    }

    private IReadOnlyList<object> GetCommandRestrictions(Type type, MethodInfo methodInfo)
    {
        var restrictions = new List<object>();

        foreach (var restrictionsFactory in _restrictionsFactories)
        {
            restrictions.AddRange(restrictionsFactory.CreateRestrictions(type, methodInfo));
        }

        return restrictions;
    }

    private static string GetCommandId(MethodInfo methodInfo)
        => $"{methodInfo.Name}({string.Join(',', methodInfo.GetParameters().Select(p => p.Name))})".ToLowerInvariant();
}