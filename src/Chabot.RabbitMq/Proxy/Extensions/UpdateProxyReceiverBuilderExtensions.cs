using Chabot.Proxy;
using Chabot.RabbitMq.Proxy;

// ReSharper disable once CheckNamespace
namespace Chabot.RabbitMq;

public static class UpdateProxyReceiverBuilderExtensions
{
    public static IUpdateProxyReceiverBuilder<TUpdate> UseRabbitMq<TUpdate>(
        this IUpdateProxyReceiverBuilder<TUpdate> builder,
        Action<RabbitMqUpdateProxyReceiverBuilder<TUpdate>> configureBuilder)
    {
        var listenerBuilder = new RabbitMqUpdateProxyReceiverBuilder<TUpdate>(builder);

        configureBuilder(listenerBuilder);

        listenerBuilder.Register();

        return builder;
    }
}