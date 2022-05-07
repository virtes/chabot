namespace Chabot.State;

public interface IStateWriter<in TUserId>
{
    Task<UserState> WriteState(IState? state, TUserId userId);
}