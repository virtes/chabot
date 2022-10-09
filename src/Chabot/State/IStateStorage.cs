namespace Chabot.State;

public interface IStateStorage<in TUserId, TSerializedState>
{
    ValueTask WriteState(TUserId userId, TSerializedState state);

    ValueTask<TSerializedState?> ReadState(TUserId userId);
}