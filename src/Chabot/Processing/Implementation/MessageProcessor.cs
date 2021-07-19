using System.Threading.Tasks;
using Chabot.Configuration;
using Chabot.Messages;

namespace Chabot.Processing.Implementation
{
    public class MessageProcessor<TMessage> : IMessageProcessor<TMessage> where TMessage : IMessage
    {
        private readonly IMessageContextFactory<TMessage> _messageContextFactory;
        private readonly IMessageProcessingConfiguration<TMessage> _configuration;

        public MessageProcessor(IMessageContextFactory<TMessage> messageContextFactory,
            IMessageProcessingConfiguration<TMessage> configuration)
        {
            _messageContextFactory = messageContextFactory;
            _configuration = configuration;
        }

        public async ValueTask ProcessAsync(TMessage message)
        {
            using var messageContext = _messageContextFactory.CreateContext(message);

            if (_configuration.ProcessingEntryPoint != null)
            {
                var processTask = _configuration.ProcessingEntryPoint(messageContext);
                if (!processTask.IsCompletedSuccessfully)
                    await processTask;
            }
        }
    }
}