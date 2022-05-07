namespace Chabot.State;

public interface IStateStorage<in TUserId, TSerializedState>
{
    Task WriteState(TUserId userId, TSerializedState state);

    ValueTask<TSerializedState?> ReadState(TUserId userId);
}