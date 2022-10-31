using Chabot.State;

namespace Chabot.Message;

public class MessageContext<TMessage, TUser>
{
    public MessageContext(IServiceProvider services, TMessage message, TUser user)
    {
        Services = services;
        Message = message;
        User = user;
    }

    public TMessage Message { get; }

    public TUser User { get; }

    public UserState UserState { get; set; } = default!;

    public IServiceProvider Services { get; }
}