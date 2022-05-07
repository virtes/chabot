using Chabot.User;

namespace Chabot.Message;

public interface IMessageHandler<in TMessage, in TUser, in TId>
    where TUser : IUser<TId>
{
    Task HandleMessage(TMessage message, TUser user);
}