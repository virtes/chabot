using Chabot.Message;
using Chabot.User;

namespace Chabot.Proxy.RabbitMq;

public class ChabotRabbitMqMessage<TMessage, TUser, TUserId>
    where TMessage : IMessage
    where TUser : IUser<TUserId>
{
    public TMessage Message { get; set; } = default!;

    public TUser User { get; set; } = default!;
}