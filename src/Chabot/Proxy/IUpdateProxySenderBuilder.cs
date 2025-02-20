namespace Chabot.Proxy;

public interface IUpdateProxySenderBuilder<TUpdate>
{
    IChabotBuilder<TUpdate> ChabotBuilder { get; }

    void Register<TSerializedUpdate>();
}