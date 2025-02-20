namespace Chabot.State;

public interface IStateStorage<TSerializedState>
{
    ValueTask<SerializedState<TSerializedState>?> GetState(string stateKey, string stateTargetType);

    ValueTask SetState(string stateKey, string stateTargetType, SerializedState<TSerializedState>? serializedState);
}