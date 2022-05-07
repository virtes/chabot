using Chabot.User;

namespace Chabot.Message;

public interface IMiddleware<TMessage, TUser, TUserId>
    where TMessage : IMessage
    where TUser : IUser<TUserId>
{
    Task Invoke(MessageContext<TMessage, TUser, TUserId> messageContext, 
        HandleMessage<TMessage, TUser, TUserId> next);
}