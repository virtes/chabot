using Chabot.Messages;
using Chabot.Processing.Implementation;

namespace Chabot.Configuration.Implementation
{
    public class MessageProcessingConfiguration<TMessage> : IMessageProcessingConfiguration<TMessage>
        where TMessage : IMessage
    {
        public MessageProcessingConfiguration(ProcessingDelegate<TMessage> processingEntryPoint)
        {
            ProcessingEntryPoint = processingEntryPoint;
        }

        public ProcessingDelegate<TMessage> ProcessingEntryPoint { get; }
    }
}