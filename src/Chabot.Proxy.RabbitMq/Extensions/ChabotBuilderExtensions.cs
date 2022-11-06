using Chabot.Configuration;
using Chabot.Proxy.RabbitMq.Configuration;
using Chabot.Proxy.RabbitMq.Implementation;
using EasyNetQ;
using EasyNetQ.Consumer;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

// ReSharper disable once CheckNamespace
namespace Chabot.Proxy.RabbitMq;

public static class ChabotBuilderExtensions
{
    [UsedImplicitly]
    public static ChabotBuilder<TMessage, TUser, TStateTarget>
        UseRabbitMqListenerProxy<TMessage, TUser, TStateTarget>(
            this ChabotBuilder<TMessage, TUser, TStateTarget> chabotBuilder,
            Action<RabbitMqProxyOptions> configureOptions)
    {
        chabotBuilder.Services.TryAddSingleton<DefaultSendReceive>();
        chabotBuilder.Services.TryAddSingleton<ISendReceive, SendReceiveTraceContextWrapper>();

        AddProxyOptions(chabotBuilder.Services, configureOptions);
        RegisterEasyNetQ(chabotBuilder.Services);

        chabotBuilder.Services.AddScoped<RabbitMqMessageProducerMiddleware<TMessage, TUser>>();
        chabotBuilder.UseMiddleware<RabbitMqMessageProducerMiddleware<TMessage, TUser>>();

        return chabotBuilder;
    }

    [UsedImplicitly]
    public static ChabotBuilder<TMessage, TUser, TStateTarget>
        UseRabbitMqWorkerProxy<TMessage, TUser, TStateTarget>(
            this ChabotBuilder<TMessage, TUser, TStateTarget> chabotBuilder,
            Action<RabbitMqProxyOptions> configureOptions)
    {
        chabotBuilder.Services.TryAddSingleton<IHandlerRunner, TraceContextExtractorHandlerRunner>();

        AddProxyOptions(chabotBuilder.Services, configureOptions);
        RegisterEasyNetQ(chabotBuilder.Services);

        chabotBuilder.Services.AddHostedService<ReceiveMessagesQueueHostedService<TMessage, TUser>>();

        return chabotBuilder;
    }

    private static void AddProxyOptions(IServiceCollection services,
        Action<RabbitMqProxyOptions> configureOptions)
    {
        services
            .AddOptions<RabbitMqProxyOptions>()
            .Configure(configureOptions)
            .ValidateDataAnnotations()
            .ValidateOnStart();
    }

    private static void RegisterEasyNetQ(IServiceCollection services)
    {
        services.RegisterEasyNetQ(sr =>
        {
            var options = sr.Resolve<IOptions<RabbitMqProxyOptions>>().Value;

            return new ConnectionConfiguration
            {
                Hosts = options.Hosts
                    .Select(h => new HostConfiguration
                    {
                        Host = h.Host,
                        Port = h.Port
                    })
                    .ToList()
            };
        }, sr => sr.EnableSerilogLogging());
    }
}