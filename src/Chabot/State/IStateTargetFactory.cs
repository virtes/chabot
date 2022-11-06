namespace Chabot.State;

public interface IStateTargetFactory<in TMessage, in TUser, out TStateTarget>
{
    TStateTarget GetStateTarget(TMessage message, TUser user);
}