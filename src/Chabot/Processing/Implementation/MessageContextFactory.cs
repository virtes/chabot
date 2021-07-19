using Chabot.Messages;
using Microsoft.Extensions.DependencyInjection;

namespace Chabot.Processing.Implementation
{
    public class MessageContextFactory<TMessage> : IMessageContextFactory<TMessage>
        where TMessage : IMessage
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public MessageContextFactory(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public IMessageContext<TMessage> CreateContext(TMessage message)
        {
            var serviceScope = _serviceScopeFactory.CreateScope();

            return new MessageContext<TMessage>(message, serviceScope);
        }
    }
}