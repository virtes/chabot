using System.Text.Json;
using Chabot.Proxy;
using Chabot.SystemTextJson.Proxy;

// ReSharper disable once CheckNamespace
namespace Chabot.SystemTextJson;

public static class SerializableUpdateProxyBuilderExtensions
{
    public static ISerializableUpdateProxyBuilder<TUpdate, byte[]> UseSystemTextJsonUpdateSerializer<TUpdate>(
        this ISerializableUpdateProxyBuilder<TUpdate, byte[]> builder,
        JsonSerializerOptions? jsonSerializerOptions = null)
    {
        builder.UseSerializer(_ => new SystemTextJsonUpdateSerializer<TUpdate>(jsonSerializerOptions));

        return builder;
    }
}