namespace Chabot.State;

public interface IStateStorage<in TMessage, in TUser, TSerializedState>
{
    ValueTask WriteState(TMessage message, TUser user, TSerializedState state);

    ValueTask<TSerializedState?> ReadState(TMessage message, TUser user);
}