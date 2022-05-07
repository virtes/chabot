using Chabot.Configuration;
using Chabot.Configuration.Exceptions;
using Chabot.KafkaProxy.Configuration;
using Chabot.KafkaProxy.Implementation;
using Chabot.Message;
using Chabot.User;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Chabot.KafkaProxy;

public static class ChabotBuilderExtensions
{
    public static ChabotBuilder<TMessage, TUser, TUserId> UseKafkaProxyProducer<TMessage, TUser, TUserId>(
        this ChabotBuilder<TMessage, TUser, TUserId> chabotBuilder,
        Action<ChabotKafkaProxyProducerOptions> configureOptions)
        where TMessage : IMessage
        where TUser : IUser<TUserId>
    {
        if (chabotBuilder.Services.FirstOrDefault(sd => sd.ServiceType == typeof(IMessageHandler<TMessage, TUser, TUserId>)) is var descriptor
            // ReSharper disable once MergeIntoPattern
            && descriptor is not null)
        {
            throw new InvalidChabotConfigurationException(
                $"{typeof(IMessageHandler<TMessage, TUser, TUserId>)} already registered ({descriptor.ServiceType.FullName})");
        }

        chabotBuilder.Services
            .AddOptions<ChabotKafkaProxyProducerOptions>()
            .Configure(configureOptions)
            .ValidateDataAnnotations();

        chabotBuilder.Services.AddSingleton<IMessageHandler<TMessage, TUser, TUserId>,
            KafkaProxyProducerMessageHandler<TMessage, TUser, TUserId>>();

        return chabotBuilder;
    }

    public static ChabotBuilder<TMessage, TUser, TUserId> UseKafkaProxyConsumer<TMessage, TUser, TUserId>(
        this ChabotBuilder<TMessage, TUser, TUserId> chabotBuilder,
        Action<ChabotKafkaProxyConsumerOptions> configureOptions)
        where TMessage : IMessage
        where TUser : IUser<TUserId>
    {
        chabotBuilder.Services
            .AddOptions<ChabotKafkaProxyConsumerOptions>()
            .Configure(configureOptions)
            .ValidateDataAnnotations();

        chabotBuilder.Services.AddHostedService<ChabotMessageKafkaConsumerHostedService<TMessage, TUser, TUserId>>();
        chabotBuilder.Services.AddTransient<ChabotMessageKafkaConsumer<TMessage, TUser, TUserId>>();

        return chabotBuilder;
    }
}