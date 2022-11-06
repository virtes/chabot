namespace Chabot.State;

public interface IStateReader<in TMessage, in TUser, in TStateTarget>
{
    Task<UserState> ReadState(TMessage message,
        TUser user, TStateTarget stateTarget);
}