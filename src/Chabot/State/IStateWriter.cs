namespace Chabot.State;

public interface IStateWriter<in TStateTarget>
{
    Task<UserState> WriteState(IState state, TStateTarget stateTarget);
}