namespace Chabot.Proxy;

public interface IUpdateProxyReceiverBuilder<TUpdate>
{
    IChabotBuilder<TUpdate> ChabotBuilder { get; }

    void Register<TSerializedUpdate>();
}