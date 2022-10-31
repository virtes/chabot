using Chabot.Message;
using Chabot.Proxy.RabbitMq.Configuration;
using EasyNetQ;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chabot.Proxy.RabbitMq.Implementation;

public class RabbitMqMessageProducerMiddleware<TMessage, TUser>
    : IMiddleware<TMessage, TUser>
{
    private readonly ISendReceive _sendReceive;
    private readonly ILogger<RabbitMqMessageProducerMiddleware<TMessage, TUser>> _logger;
    private readonly RabbitMqProxyOptions _options;

    public RabbitMqMessageProducerMiddleware(ISendReceive sendReceive,
        IOptions<RabbitMqProxyOptions> options,
        ILogger<RabbitMqMessageProducerMiddleware<TMessage, TUser>> logger)
    {
        _sendReceive = sendReceive;
        _logger = logger;
        _options = options.Value;
    }

    public async Task Invoke(MessageContext<TMessage, TUser> messageContext,
        HandleMessage<TMessage, TUser> next)
    {
        var chabotMessage = new ChabotRabbitMqMessage<TMessage, TUser>
        {
            Message = messageContext.Message,
            User = messageContext.User
        };

        await _sendReceive.SendAsync(_options.QueueName, chabotMessage);
        _logger.LogInformation("Message sent to rabbit mq");

        await next(messageContext);
    }
}