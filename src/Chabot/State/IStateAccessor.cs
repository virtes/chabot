namespace Chabot.State;

public interface IStateAccessor
{
    ValueTask<object?> GetState(IStateTarget stateTarget);

    ValueTask SetState(IStateTarget stateTarget, object? state);
}