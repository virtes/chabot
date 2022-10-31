namespace Chabot.State;

public interface IStateReader<in TMessage, in TUser>
{
    Task<UserState> ReadState(TMessage message, TUser user);
}