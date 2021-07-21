using Chabot.Messages;
using Chabot.Processing;

namespace Chabot.Commands.Implementation
{
    public abstract class CommandGroupBase<TMessage>
        where TMessage : IMessage
    {
        public IMessageContext<TMessage> Context { get; internal set; } = default!;
    }
}