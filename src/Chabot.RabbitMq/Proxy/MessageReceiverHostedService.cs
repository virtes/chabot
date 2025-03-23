using Chabot.Proxy;
using Chabot.RabbitMq.Configuration;
using EasyNetQ;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Chabot.RabbitMq.Proxy;

internal class MessageReceiverHostedService : IHostedService
{
    private readonly ISendReceive _sendReceive;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly RabbitMqProxyOptions _options;
    private IDisposable? _receiveDisposable;

    public MessageReceiverHostedService(
        ISendReceive sendReceive,
        IOptions<RabbitMqProxyOptions> options,
        IServiceScopeFactory serviceScopeFactory)
    {
        _sendReceive = sendReceive;
        _serviceScopeFactory = serviceScopeFactory;
        _options = options.Value;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _receiveDisposable = await _sendReceive.ReceiveAsync<byte[]>(
            _options.Queue, async m =>
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var receiver = scope.ServiceProvider.GetRequiredService<IUpdateProxyReceiver<byte[]>>();

                await receiver.UpdateReceived(m);
            },
            c => c
                .WithPrefetchCount(_options.PrefetchCount)
                .WithSingleActiveConsumer(_options.SingleActiveConsumer));
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _receiveDisposable?.Dispose();

        return Task.CompletedTask;
    }
}