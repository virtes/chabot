using Chabot.Message;
using Chabot.Proxy.RabbitMq.Configuration;
using Chabot.User;
using EasyNetQ;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using IMessage = Chabot.Message.IMessage;

namespace Chabot.Proxy.RabbitMq.Implementation;

public class ReceiveMessagesQueueHostedService<TMessage, TUser, TUserId> : IHostedService
    where TUser : IUser<TUserId> where TMessage : IMessage
{
    private readonly ISendReceive _sendReceive;
    private readonly IMessageHandler<TMessage, TUser, TUserId> _messageHandler;
    private readonly RabbitMqProxyOptions _options;
    private IDisposable? _receiveDisposable;

    public ReceiveMessagesQueueHostedService(
        ISendReceive sendReceive,
        IOptions<RabbitMqProxyOptions> options,
        IMessageHandler<TMessage, TUser, TUserId> messageHandler)
    {
        _sendReceive = sendReceive;
        _messageHandler = messageHandler;
        _options = options.Value;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _receiveDisposable = await _sendReceive.ReceiveAsync<ChabotRabbitMqMessage<TMessage, TUser, TUserId>>(
            _options.QueueName, async m =>
            {
                await _messageHandler.HandleMessage(m.Message, m.User);
            });
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _receiveDisposable?.Dispose();

        return Task.CompletedTask;
    }
}