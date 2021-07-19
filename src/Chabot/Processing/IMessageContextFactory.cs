using Chabot.Messages;

namespace Chabot.Processing
{
    public interface IMessageContextFactory<TMessage>
        where TMessage : IMessage
    {
        IMessageContext<TMessage> CreateContext(TMessage message);
    }
}