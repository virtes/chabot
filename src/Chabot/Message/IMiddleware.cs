namespace Chabot.Message;

public interface IMiddleware<TMessage, TUser>
{
    Task Invoke(MessageContext<TMessage, TUser> messageContext,
        HandleMessage<TMessage, TUser> next);
}