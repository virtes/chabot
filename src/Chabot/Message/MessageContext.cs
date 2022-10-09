using Chabot.State;
using Chabot.User;

namespace Chabot.Message;

public class MessageContext<TMessage, TUser, TUserId>
    where TUser : IUser<TUserId>
    where TMessage : IMessage
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