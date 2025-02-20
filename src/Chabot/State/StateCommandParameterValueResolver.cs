using System.Reflection;
using Chabot.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace Chabot.State;

internal class StateCommandParameterValueResolver<TUpdate> : ICommandParameterValueResolver<TUpdate>
{
    private readonly string _stateTargetType;

    public StateCommandParameterValueResolver(string stateTargetType)
    {
        _stateTargetType = stateTargetType;
    }

    public async ValueTask<object?> ResolveParameterValue(ParameterInfo parameterInfo, UpdateContext<TUpdate> updateContext)
    {
        var stateTargetResolverFactory = updateContext.ServiceProvider.GetRequiredService<IStateTargetResolverFactory<TUpdate>>();
        var stateAccessor = updateContext.ServiceProvider.GetRequiredService<IStateAccessor>();

        var stateTargetResolver = stateTargetResolverFactory.CreateStateTargetResolver(_stateTargetType);
        if (!stateTargetResolver.TryResolveStateTarget(updateContext.Update, out var stateTarget))
            return null;

        var state = await stateAccessor.GetState(stateTarget);
        if (state is null)
            return null;

        if (state.GetType() != parameterInfo.ParameterType)
            return null;

        return state;
    }
}