using Chabot.Commands;
using Chabot.Messages;

namespace Chabot.Routing
{
    public interface IMessageRouter<in TMessage>
        where TMessage : IMessage
    {
        CommandInfo RouteMessage(TMessage message);
    }
}