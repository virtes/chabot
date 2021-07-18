using Chabot.Messages;
using Chabot.Processing.Implementation;

namespace Chabot.Configuration
{
    public interface IMessageProcessingConfiguration<TMessage>
        where TMessage : IMessage
    {
        ProcessingDelegate<TMessage> ProcessingEntryPoint { get; }
    }
}