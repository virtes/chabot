using Chabot.Proxy;
using Chabot.RabbitMq.Proxy;

// ReSharper disable once CheckNamespace
namespace Chabot.RabbitMq;

public static class UpdateProxySenderBuilderExtensions
{
    public static IUpdateProxySenderBuilder<TUpdate> UseRabbitMq<TUpdate>(
        this IUpdateProxySenderBuilder<TUpdate> builder,
        Action<RabbitMqUpdateProxySenderBuilder<TUpdate>> configureKafkaProxyBuilder)
    {
        builder.Register<byte[]>();

        var kafkaProxySenderBuilder = new RabbitMqUpdateProxySenderBuilder<TUpdate>(builder);
        configureKafkaProxyBuilder(kafkaProxySenderBuilder);

        kafkaProxySenderBuilder.Register();

        return builder;
    }
}