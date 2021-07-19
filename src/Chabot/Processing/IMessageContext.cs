using System;
using Chabot.Messages;
using Chabot.User;

namespace Chabot.Processing
{
    public interface IMessageContext<out TMessage> : IDisposable
        where TMessage : IMessage
    {
        TMessage Message { get; }

        IServiceProvider Services { get; }

        UserIdentity User { get; set; }
    }
}