using Chabot.Message;

namespace Chabot.Command;

public interface IMessageAction<TMessage, TUser>
{
    Task Execute(MessageContext<TMessage, TUser> messageContext);
}