using System;
using Chabot.Messages;
using Chabot.User;

// ReSharper disable once CheckNamespace
namespace Chabot.Processing
{
    public class MessageContext<TMessage> where TMessage : IMessage
    {
        public MessageContext(TMessage message, IServiceProvider services)
        {
            Message = message;
            Services = services;
            User = null!;
        }

        public TMessage Message { get; }

        public IServiceProvider Services { get; }

        public UserIdentity User { get; set; }
    }
}