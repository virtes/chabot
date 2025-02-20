using Chabot.Commands;

namespace Chabot.State;

internal class StateCommandRestrictionHandler<TUpdate> : ICommandRestrictionHandler<TUpdate, StateRestriction>
{
    private readonly IStateTargetResolverFactory<TUpdate> _stateTargetResolverFactory;
    private readonly IStateAccessor _stateAccessor;

    public StateCommandRestrictionHandler(
        IStateTargetResolverFactory<TUpdate> stateTargetResolverFactory,
        IStateAccessor stateAccessor)
    {
        _stateTargetResolverFactory = stateTargetResolverFactory;
        _stateAccessor = stateAccessor;
    }

    public async ValueTask<bool> IsAllowed(TUpdate update, UpdateMetadata updateMetadata,
        StateRestriction restriction)
    {
        var stateTargetResolver = _stateTargetResolverFactory.CreateStateTargetResolver(
            restriction.StateTargetType);

        if (!stateTargetResolver.TryResolveStateTarget(update, out var stateTarget))
            return false;

        var state = await _stateAccessor.GetState(stateTarget);
        if (state is null)
            return restriction.AllowEmptyState;

        var stateType = state.GetType();
        return restriction.AllowedStateTypes.Any(t => t == stateType);
    }
}