namespace Chabot.Message;

public interface IMessageHandler<in TMessage, in TUser>
{
    Task HandleMessage(TMessage message, TUser user);
}