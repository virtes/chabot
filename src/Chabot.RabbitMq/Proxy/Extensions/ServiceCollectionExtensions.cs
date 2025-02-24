using Chabot.RabbitMq.Configuration;
using EasyNetQ;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Chabot.RabbitMq.Proxy.Extensions;

public static class ServiceCollectionExtensions
{
    internal static void RegisterEasyNetQ(this IServiceCollection services,
        Action<RabbitMqProxyOptions> configureOptions)
    {
        services.BindOptions(configureOptions);
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
                    .ToList(),
                UserName = options.Username,
                Password = options.Password
            };
        }, sr => sr.EnableSerilogLogging());
    }
}