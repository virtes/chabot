using System.Reflection;

namespace Chabot.Commands;

internal class FromMessageTextParameterValueResolver<TUpdate> : ICommandParameterValueResolver<TUpdate>
{
    public ValueTask<object?> ResolveParameterValue(ParameterInfo parameterInfo,
        UpdateContext<TUpdate> updateContext)
    {
        var text = updateContext.UpdateMetadata.Properties.GetValueOrDefault(UpdateProperties.MessageText);
        return ValueTask.FromResult<object?>(text);
    }
}