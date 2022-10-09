namespace Chabot.State.Implementation;

public class DefaultStateFactory<TUserId> : IDefaultStateFactory<TUserId>
{
    public IState CreateDefaultState(TUserId userId)
    {
        return DefaultState.Instance;
    }
}