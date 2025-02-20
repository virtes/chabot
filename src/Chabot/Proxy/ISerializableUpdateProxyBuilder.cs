namespace Chabot.Proxy;

public interface ISerializableUpdateProxyBuilder<TUpdate, TSerializedUpdate>
{
    void UseSerializer(Func<IServiceProvider, IUpdateSerializer<TUpdate, TSerializedUpdate>> serializerFactory);
}