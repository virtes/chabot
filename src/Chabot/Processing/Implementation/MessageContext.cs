using System;
using Chabot.Messages;
using Chabot.User;
using Microsoft.Extensions.DependencyInjection;

namespace Chabot.Processing.Implementation
{
    public class MessageContext<TMessage> : IMessageContext<TMessage>
        where TMessage : IMessage
    {
        private readonly IServiceScope _serviceScope;

        public MessageContext(TMessage message, IServiceScope serviceScope)
        {
            _serviceScope = serviceScope;
            Message = message;
            User = null!;
        }

        public TMessage Message { get; }

        public IServiceProvider Services => _serviceScope.ServiceProvider;

        public UserIdentity User { get; set; }

        public void Dispose()
        {
            _serviceScope.Dispose();
        }
    }
}