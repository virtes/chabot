namespace Chabot.State;

public interface IStateTargetResolver<in TUpdate>
{
    bool TryResolveStateTarget(TUpdate update, out IStateTarget stateTarget);
}