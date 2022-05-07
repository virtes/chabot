using System.Text.Json;
using Confluent.Kafka;

namespace Chabot.KafkaProxy.Implementation;

public class SystemTextJsonSerializer<T> : ISerializer<T>, IDeserializer<T>
{
    public byte[] Serialize(T data, SerializationContext context)
    {
        return JsonSerializer.SerializeToUtf8Bytes(data);
    }

    public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        if (isNull)
            return default!;

        return JsonSerializer.Deserialize<T>(data)!;
    }
}