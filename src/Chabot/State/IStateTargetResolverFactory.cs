namespace Chabot.State;

public interface IStateTargetResolverFactory<in TUpdate>
{
    IStateTargetResolver<TUpdate> CreateStateTargetResolver(string stateTargetType);
}