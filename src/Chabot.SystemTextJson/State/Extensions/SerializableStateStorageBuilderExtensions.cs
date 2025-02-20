using System.Text.Json;
using Chabot.State;
using Chabot.SystemTextJson.State;

// ReSharper disable once CheckNamespace
namespace Chabot.SystemTextJson;

public static class SerializableStateStorageBuilderExtensions
{
    public static ISerializableStateStorageBuilder<byte[]> UseSystemTextJsonStateSerializer(
        this ISerializableStateStorageBuilder<byte[]> builder,
        JsonSerializerOptions? jsonSerializerOptions = null)
    {
        builder.UseSerializer(_ => new SystemTextJsonStateSerializer(jsonSerializerOptions));

        return builder;
    }

    public static ISerializableStateStorageBuilder<string> UseSystemTextJsonStateSerializer(
        this ISerializableStateStorageBuilder<string> builder,
        JsonSerializerOptions? jsonSerializerOptions = null)
    {
        builder.UseSerializer(_ => new SystemTextJsonStateSerializer(jsonSerializerOptions));

        return builder;
    }
}