using Chabot.Messages;
using Chabot.Processing;

// ReSharper disable once CheckNamespace
namespace Chabot.Commands
{
    public abstract class CommandGroupBase<TMessage>
        where TMessage : IMessage
    {
        public IMessageContext<TMessage> Context { get; internal set; } = default!;
    }
}