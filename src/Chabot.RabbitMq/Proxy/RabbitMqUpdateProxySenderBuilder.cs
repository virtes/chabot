using Chabot.Exceptions;
using Chabot.Proxy;
using Chabot.RabbitMq.Configuration;
using Chabot.RabbitMq.Proxy.Extensions;
using EasyNetQ;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Chabot.RabbitMq.Proxy;

public class RabbitMqUpdateProxySenderBuilder<TUpdate> : ISerializableUpdateProxyBuilder<TUpdate, byte[]>
{
    private readonly IUpdateProxySenderBuilder<TUpdate> _proxySenderBuilder;

    public RabbitMqUpdateProxySenderBuilder(IUpdateProxySenderBuilder<TUpdate> proxySenderBuilder)
    {
        _proxySenderBuilder = proxySenderBuilder;
    }

    private Func<IServiceProvider, IUpdateSerializer<TUpdate, byte[]>>? _serializerFactory;
    private Action<RabbitMqProxyOptions>? _configureRabbitMqProxy;

    [PublicAPI]
    public void UseSerializer(Func<IServiceProvider, IUpdateSerializer<TUpdate, byte[]>> serializerFactory)
    {
        _serializerFactory = serializerFactory;
    }

    [PublicAPI]
    public void Configure(Action<RabbitMqProxyOptions> configure)
    {
        _configureRabbitMqProxy = configure;
    }

    internal void Register()
    {
        if (_serializerFactory is null)
            throw new InvalidChabotConfigurationException("RabbitMq update proxy serializer must be specified");

        if (_configureRabbitMqProxy is null)
            throw new InvalidChabotConfigurationException("RabbitMq proxy must be configured");

        // ReSharper disable once RedundantTypeArgumentsOfMethod
        _proxySenderBuilder.ChabotBuilder.Services.TryAddScoped<IUpdateSerializer<TUpdate, byte[]>>(_serializerFactory);
        _proxySenderBuilder.ChabotBuilder.Services.TryAddScoped<IUpdateProxySender<byte[]>, RabbitMqUpdateProxySender>();


        _proxySenderBuilder.ChabotBuilder.Services.TryAddSingleton<DefaultSendReceive>();
        _proxySenderBuilder.ChabotBuilder.Services.TryAddSingleton<ISendReceive, SendReceiveTraceContextWrapper>();

        _proxySenderBuilder.ChabotBuilder.Services.RegisterEasyNetQ(_configureRabbitMqProxy);
    }
}