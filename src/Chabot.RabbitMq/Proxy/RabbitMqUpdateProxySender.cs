using Chabot.Proxy;
using Chabot.RabbitMq.Configuration;
using EasyNetQ;
using Microsoft.Extensions.Options;

namespace Chabot.RabbitMq.Proxy;

internal class RabbitMqUpdateProxySender : IUpdateProxySender<byte[]>
{
    private readonly ISendReceive _sendReceive;
    private readonly IOptions<RabbitMqProxyOptions> _options;

    public RabbitMqUpdateProxySender(ISendReceive sendReceive,
        IOptions<RabbitMqProxyOptions> options)
    {
        _sendReceive = sendReceive;
        _options = options;
    }

    public async Task SendUpdate(byte[] partitionKey, byte[] serializedUpdate)
    {
        await _sendReceive.SendAsync(_options.Value.Queue, serializedUpdate);
    }
}