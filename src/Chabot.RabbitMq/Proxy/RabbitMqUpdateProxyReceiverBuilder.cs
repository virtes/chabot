using Chabot.Exceptions;
using Chabot.Proxy;
using Chabot.RabbitMq.Configuration;
using Chabot.RabbitMq.Proxy.Extensions;
using EasyNetQ.Consumer;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Chabot.RabbitMq.Proxy;

public class RabbitMqUpdateProxyReceiverBuilder<TUpdate> : ISerializableUpdateProxyBuilder<TUpdate, byte[]>
{
    public IUpdateProxyReceiverBuilder<TUpdate> ChabotProxyReceiverBuilder { get; }

    private Func<IServiceProvider, IUpdateSerializer<TUpdate, byte[]>>? _serializerFactory;
    private Action<RabbitMqProxyOptions>? _configureRabbitMqProxy;

    public RabbitMqUpdateProxyReceiverBuilder(IUpdateProxyReceiverBuilder<TUpdate> chabotProxyReceiverBuilder)
    {
        ChabotProxyReceiverBuilder = chabotProxyReceiverBuilder;
    }

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
        ChabotProxyReceiverBuilder.ChabotBuilder.Services.TryAddScoped<IUpdateSerializer<TUpdate, byte[]>>(_serializerFactory);
        ChabotProxyReceiverBuilder.Register<byte[]>();

        ChabotProxyReceiverBuilder.ChabotBuilder.Services.TryAddSingleton<IHandlerRunner, TraceContextExtractorHandlerRunner>();
        ChabotProxyReceiverBuilder.ChabotBuilder.Services.AddHostedService<MessageReceiverHostedService>();

        ChabotProxyReceiverBuilder.ChabotBuilder.Services.RegisterEasyNetQ(_configureRabbitMqProxy);
    }
}