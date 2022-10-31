namespace Chabot.State.Implementation;

public class DefaultStateFactory<TMessage, TUser> : IDefaultStateFactory<TMessage, TUser>
{
    public IState CreateDefaultState(TMessage message, TUser user)
    {
        return DefaultState.Instance;
    }
}