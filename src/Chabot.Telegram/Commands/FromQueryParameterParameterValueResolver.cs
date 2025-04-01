using System.Reflection;
using System.Web;
using Chabot.Commands;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Chabot.Telegram.Commands;

internal class FromQueryParameterParameterValueResolver : ICommandParameterValueResolver<Update>
{
    private readonly object? _defaultValue;
    private readonly string _key;
    private readonly Func<string?, object?> _valueParser;

    public FromQueryParameterParameterValueResolver(
        object? defaultValue,
        string key,
        Func<string?, object?> valueParser)
    {
        _defaultValue = defaultValue;
        _key = key;
        _valueParser = valueParser;
    }

    public ValueTask<object?> ResolveParameterValue(
        ParameterInfo parameterInfo, UpdateContext<Update> updateContext)
    {
        if (updateContext.Update.Type != UpdateType.CallbackQuery)
            throw new InvalidOperationException("Only CallbackQuery update is supported");

        var payloadData = updateContext.Update.CallbackQuery!.Data!;
        var queryParameters = payloadData.Split('?', StringSplitOptions.RemoveEmptyEntries);
        if (queryParameters.Length < 2)
            return ValueTask.FromResult(_defaultValue);

        var parameters = HttpUtility.ParseQueryString(queryParameters[1]);
        var parameterValue = parameters.Get(_key);
        if (string.IsNullOrEmpty(parameterValue))
            return ValueTask.FromResult(_defaultValue);

        return ValueTask.FromResult(_valueParser(parameterValue));
    }
}