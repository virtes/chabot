namespace Chabot.State;

public interface IStateWriter<in TMessage, in TUser>
{
    Task<UserState> WriteState(IState state, TMessage message, TUser user);
}