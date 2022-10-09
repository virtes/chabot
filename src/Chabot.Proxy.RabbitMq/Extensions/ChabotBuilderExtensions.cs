using Chabot.Configuration;
using Chabot.Proxy.RabbitMq.Configuration;
using Chabot.Proxy.RabbitMq.Implementation;
using Chabot.User;
using EasyNetQ;
using EasyNetQ.Consumer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using IMessage = Chabot.Message.IMessage;

// ReSharper disable once CheckNamespace
namespace Chabot.Proxy.RabbitMq;

public static class ChabotBuilderExtensions
{
    public static ChabotBuilder<TMessage, TUser, TUserId>
        UseRabbitMqListenerProxy<TMessage, TUser, TUserId>(
            this ChabotBuilder<TMessage, TUser, TUserId> chabotBuilder,
            Action<OptionsBuilder<RabbitMqProxyOptions>> configureOptionsBuilder)
        where TMessage : IMessage
        where TUser : IUser<TUserId>
    {
        chabotBuilder.Services.TryAddSingleton<DefaultSendReceive>();
        chabotBuilder.Services.TryAddSingleton<ISendReceive, SendReceiveTraceContextWrapper>();

        AddProxyOptions(chabotBuilder.Services, configureOptionsBuilder);
        RegisterEasyNetQ(chabotBuilder.Services);

        chabotBuilder.Services.AddScoped<RabbitMqMessageProducerMiddleware<TMessage, TUser, TUserId>>();
        chabotBuilder.UseMiddleware<RabbitMqMessageProducerMiddleware<TMessage, TUser, TUserId>>();

        return chabotBuilder;
    }

    public static ChabotBuilder<TMessage, TUser, TUserId>
        UseRabbitMqWorkerProxy<TMessage, TUser, TUserId>(
            this ChabotBuilder<TMessage, TUser, TUserId> chabotBuilder,
            Action<OptionsBuilder<RabbitMqProxyOptions>> configureOptionsBuilder)
        where TMessage : IMessage
        where TUser : IUser<TUserId>
    {
        chabotBuilder.Services.TryAddSingleton<IHandlerRunner, TraceContextExtractorHandlerRunner>();

        AddProxyOptions(chabotBuilder.Services, configureOptionsBuilder);
        RegisterEasyNetQ(chabotBuilder.Services);

        chabotBuilder.Services.AddHostedService<ReceiveMessagesQueueHostedService<TMessage, TUser, TUserId>>();

        return chabotBuilder;
    }

    private static void AddProxyOptions(IServiceCollection services,
        Action<OptionsBuilder<RabbitMqProxyOptions>> configureOptionsBuilder)
    {
        var optionsBuilder = services
            .AddOptions<RabbitMqProxyOptions>()
            .ValidateDataAnnotations();
        configureOptionsBuilder(optionsBuilder);
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