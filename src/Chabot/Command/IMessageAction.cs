using Chabot.Message;
using Chabot.User;

namespace Chabot.Command;

public interface IMessageAction<TMessage, TUser, TUserId>
    where TMessage : IMessage
    where TUser : IUser<TUserId>
{
    Task Execute(MessageContext<TMessage, TUser, TUserId> messageContext);
}