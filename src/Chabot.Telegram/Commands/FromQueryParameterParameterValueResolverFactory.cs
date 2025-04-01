using System.Reflection;
using Chabot.Commands;
using Telegram.Bot.Types;

namespace Chabot.Telegram.Commands;

internal class FromQueryParameterParameterValueResolverFactory : ICommandParameterValueResolverFactory<Update>
{
    public bool TryCreate(ParameterInfo parameterInfo, out ICommandParameterValueResolver<Update> resolver)
    {
        var fromQueryParameterAttribute = parameterInfo.GetCustomAttribute<FromQueryParameterAttribute>();
        if (fromQueryParameterAttribute is null)
        {
            resolver = null!;
            return false;
        }

        Func<string?, object?> parser;
        object? defaultValue;
        if (parameterInfo.ParameterType == typeof(string))
        {
            parser = s => s;
            defaultValue = fromQueryParameterAttribute.DefaultValue;
        }
        else if (parameterInfo.ParameterType == typeof(int))
        {
            parser = s => int.TryParse(s, out var parsedInt) ? parsedInt : fromQueryParameterAttribute.DefaultValue;
            defaultValue = fromQueryParameterAttribute.DefaultValue ?? default(int);
        }
        else if (parameterInfo.ParameterType == typeof(bool))
        {
            parser = s =>
            {
                if (s is null)
                    return fromQueryParameterAttribute.DefaultValue;

                var sNormalized = s.ToLowerInvariant();

                return sNormalized switch
                {
                    "true" => true,
                    "false" => false,
                    _ => fromQueryParameterAttribute.DefaultValue
                };
            };
            defaultValue = fromQueryParameterAttribute.DefaultValue ?? false;
        }
        else
            throw new InvalidOperationException("Invalid parameter type");

        resolver = new FromQueryParameterParameterValueResolver(
            defaultValue: defaultValue,
            key: fromQueryParameterAttribute.Key ?? parameterInfo.Name!,
            valueParser: parser);
        return true;
    }
}