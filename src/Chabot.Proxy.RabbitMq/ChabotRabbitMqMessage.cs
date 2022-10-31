namespace Chabot.Proxy.RabbitMq;

public class ChabotRabbitMqMessage<TMessage, TUser>
{
    public TMessage Message { get; set; } = default!;

    public TUser User { get; set; } = default!;
}