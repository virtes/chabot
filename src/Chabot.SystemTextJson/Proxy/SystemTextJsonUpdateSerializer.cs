using System.Text.Json;
using Chabot.Proxy;

namespace Chabot.SystemTextJson.Proxy;

internal class SystemTextJsonUpdateSerializer<TUpdate> : IUpdateSerializer<TUpdate, byte[]>
{
    private readonly JsonSerializerOptions _options;

    public SystemTextJsonUpdateSerializer(JsonSerializerOptions? options)
    {
        _options = options ?? new JsonSerializerOptions();
    }

    public byte[] Serialize(TUpdate update)
    {
        return JsonSerializer.SerializeToUtf8Bytes(update, _options);
    }

    public TUpdate Deserialize(byte[] serializedUpdate)
    {
        return JsonSerializer.Deserialize<TUpdate>(serializedUpdate, _options)
               ?? throw new NullReferenceException("Deserialized value is null");
    }
}