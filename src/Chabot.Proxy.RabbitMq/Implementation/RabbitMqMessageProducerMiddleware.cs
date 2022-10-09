using Chabot.Message;
using Chabot.Proxy.RabbitMq.Configuration;
using Chabot.User;
using EasyNetQ;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using IMessage = Chabot.Message.IMessage;

namespace Chabot.Proxy.RabbitMq.Implementation;

public class RabbitMqMessageProducerMiddleware<TMessage, TUser, TUserId>
    : IMiddleware<TMessage, TUser, TUserId>
    where TMessage : IMessage
    where TUser : IUser<TUserId>
{
    private readonly ISendReceive _sendReceive;
    private readonly ILogger<RabbitMqMessageProducerMiddleware<TMessage, TUser, TUserId>> _logger;
    private readonly RabbitMqProxyOptions _options;

    public RabbitMqMessageProducerMiddleware(ISendReceive sendReceive,
        IOptions<RabbitMqProxyOptions> options,
        ILogger<RabbitMqMessageProducerMiddleware<TMessage, TUser, TUserId>> logger)
    {
        _sendReceive = sendReceive;
        _logger = logger;
        _options = options.Value;
    }

    public async Task Invoke(MessageContext<TMessage, TUser, TUserId> messageContext,
        HandleMessage<TMessage, TUser, TUserId> next)
    {
        var chabotMessage = new ChabotRabbitMqMessage<TMessage, TUser, TUserId>
        {
            Message = messageContext.Message,
            User = messageContext.User
        };

        await _sendReceive.SendAsync(_options.QueueName, chabotMessage);
        _logger.LogInformation("Message from {UserId} sent to rabbit mq",
            messageContext.User.Id);

        await next(messageContext);
    }
}