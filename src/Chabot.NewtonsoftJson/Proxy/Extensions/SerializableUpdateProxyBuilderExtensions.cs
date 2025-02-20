using Chabot.NewtonsoftJson.Proxy;
using Chabot.Proxy;

// ReSharper disable once CheckNamespace
namespace Chabot.NewtonsoftJson;

public static class SerializableUpdateProxyBuilderExtensions
{
    public static ISerializableUpdateProxyBuilder<TUpdate, byte[]> UseNewtonsoftJsonUpdateSerializer<TUpdate>(
        this ISerializableUpdateProxyBuilder<TUpdate, byte[]> builder)
    {
        builder.UseSerializer(_ => new NewtonsoftJsonUpdateSerializer<TUpdate>());

        return builder;
    }
}