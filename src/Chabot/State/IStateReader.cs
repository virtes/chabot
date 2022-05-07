namespace Chabot.State;

public interface IStateReader<in TUserId>
{
    Task<UserState> ReadState(TUserId userId);
}