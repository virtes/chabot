using System.Threading.Tasks;
using Chabot.Configuration;
using Chabot.Messages;
using Microsoft.Extensions.DependencyInjection;

namespace Chabot.Processing.Implementation
{
    public class MessageProcessor<T> : IMessageProcessor<T> where T : IMessage
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IMessageProcessingConfiguration<T> _configuration;

        public MessageProcessor(IServiceScopeFactory serviceScopeFactory,
            IMessageProcessingConfiguration<T> configuration)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _configuration = configuration;
        }

        public async ValueTask ProcessAsync(T message)
        {
            using var serviceScope = _serviceScopeFactory.CreateScope();

            var messageContext = new MessageContext<T>(message, serviceScope.ServiceProvider);

            var processTask = _configuration.ProcessingEntryPoint(messageContext);
            if (!processTask.IsCompletedSuccessfully)
                await processTask;
        }
    }
}