namespace Chabot.Proxy;

public interface IUpdateSerializer<TUpdate, TSerializedUpdate>
{
    TSerializedUpdate Serialize(TUpdate update);

    TUpdate Deserialize(TSerializedUpdate serializedUpdate);
}