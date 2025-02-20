namespace Chabot.Proxy;

public interface IUpdateProxySender<in TSerializedUpdate>
{
    Task SendUpdate(byte[] partitionKey, TSerializedUpdate serializedUpdate);
}