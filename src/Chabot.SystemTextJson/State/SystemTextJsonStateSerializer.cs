using System.Text.Json;
using Chabot.State;

namespace Chabot.SystemTextJson.State;

internal class SystemTextJsonStateSerializer : IStateSerializer<byte[]>, IStateSerializer<string>
{
    private readonly JsonSerializerOptions _options;

    public SystemTextJsonStateSerializer(JsonSerializerOptions? options)
    {
        _options = options ?? new JsonSerializerOptions();
    }

    public byte[] Serialize(object value)
    {
        return JsonSerializer.SerializeToUtf8Bytes(value, _options);
    }

    public object Deserialize(byte[] serializedState, Type type)
    {
        return JsonSerializer.Deserialize(serializedState, type, _options)
               ?? throw new NullReferenceException("Deserialized value is null");
    }

    string IStateSerializer<string>.Serialize(object value)
    {
        return JsonSerializer.Serialize(value);
    }

    public object Deserialize(string serializedState, Type type)
    {
        return JsonSerializer.Deserialize(serializedState, type, _options)
               ?? throw new NullReferenceException("Deserialized value is null");
    }
}