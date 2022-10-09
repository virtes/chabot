namespace Chabot.State;

public interface IDefaultStateFactory<in TUserId>
{
    public IState CreateDefaultState(TUserId userId);
}