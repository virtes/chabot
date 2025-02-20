namespace Chabot.Proxy;

public interface IUpdateProxyReceiver<in TSerializedUpdate>
{
    Task UpdateReceived(TSerializedUpdate serializedUpdate);
}