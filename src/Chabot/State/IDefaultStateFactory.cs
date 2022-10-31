namespace Chabot.State;

public interface IDefaultStateFactory<in TMessage, in TUser>
{
    public IState CreateDefaultState(TMessage message, TUser user);
}