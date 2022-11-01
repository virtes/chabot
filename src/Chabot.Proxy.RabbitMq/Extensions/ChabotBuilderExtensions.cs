using Chabot.Configuration;
using Chabot.Proxy.RabbitMq.Configuration;
using Chabot.Proxy.RabbitMq.Implementation;
using EasyNetQ;
using EasyNetQ.Consumer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

// ReSharper disable once CheckNamespace
namespace Chabot.Proxy.RabbitMq;

public static class ChabotBuilderExtensions
{
    public static ChabotBuilder<TMessage, TUser>
        UseRabbitMqListenerProxy<TMessage, TUser>(
            this ChabotBuilder<TMessage, TUser> chabotBuilder,
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

    public static ChabotBuilder<TMessage, TUser>
        UseRabbitMqWorkerProxy<TMessage, TUser>(
            this ChabotBuilder<TMessage, TUser> chabotBuilder,
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