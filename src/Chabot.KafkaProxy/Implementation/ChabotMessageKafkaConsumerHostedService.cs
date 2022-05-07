using Chabot.KafkaProxy.Configuration;
using Chabot.User;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chabot.KafkaProxy.Implementation;

public class ChabotMessageKafkaConsumerHostedService<TMessage, TUser, TUserId> : IHostedService
    where TUser : IUser<TUserId>
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger _logger;
    private readonly ChabotKafkaProxyConsumerOptions _options;
    private ChabotMessageKafkaConsumer<TMessage, TUser, TUserId>[]? _consumers;

    public ChabotMessageKafkaConsumerHostedService(
        IOptions<ChabotKafkaProxyConsumerOptions> options,
        IServiceProvider serviceProvider,
        ILogger<ChabotMessageKafkaConsumerHostedService<TMessage, TUser, TUserId>> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _options = options.Value;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var count = _options.MultiplyConsumer ?? 1;
        var consumers = new ChabotMessageKafkaConsumer<TMessage, TUser, TUserId>[count];

        for (int i = 0; i < count; i++)
        {
            consumers[i] = _serviceProvider.GetRequiredService<ChabotMessageKafkaConsumer<TMessage, TUser, TUserId>>();
        }

        _consumers = consumers;

        foreach (var consumer in _consumers)
        {
            _ = Task.Factory.StartNew(async () =>
            {
                await Task.Delay(5000, cancellationToken);

                try
                {
                    await consumer.Start();
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Unhandled exception in kafka consumer");
                    throw;
                }

            }, TaskCreationOptions.LongRunning);
        }

        _logger.LogInformation("Started {Count} chabot kafka consumers", count);

        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_consumers == null)
            return;

        try
        {
            _logger.LogInformation("Stopping {ConsumersCount} chabot kafka consumers",
                _consumers.Length);

            await Task.WhenAll(_consumers.Select(kc => kc.Stop()))
                .WaitAsync(cancellationToken);

            _logger.LogInformation("Kafka chabot consumers stopped");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception while stopping chabot kafka consumers");
        }
    }
}