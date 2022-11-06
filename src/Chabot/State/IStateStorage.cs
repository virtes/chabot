namespace Chabot.State;

public interface IStateStorage<in TStateTarget, TSerializedState>
{
    ValueTask WriteState(TStateTarget stateTarget, TSerializedState state);

    ValueTask<TSerializedState?> ReadState(TStateTarget stateTarget);
}