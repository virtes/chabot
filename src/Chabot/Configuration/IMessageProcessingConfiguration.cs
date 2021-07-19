using Chabot.Messages;
using Chabot.Processing.Implementation;

namespace Chabot.Configuration
{
    public interface IMessageProcessingConfiguration<in TMessage>
        where TMessage : IMessage
    {
        ProcessingDelegate<TMessage>? ProcessingEntryPoint { get; }
    }
}