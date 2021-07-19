using System;
using System.Threading.Tasks;
using Chabot.Messages;
using Chabot.Processing;
using Chabot.Processing.Implementation;

namespace Chabot.Authorization.Implementation
{
    public class AuthorizationMiddleware<TMessage> : IMiddleware<TMessage>
        where TMessage : IMessage
    {
        public ValueTask ExecuteAsync(IMessageContext<TMessage> context, ProcessingDelegate<TMessage> next)
        {
            throw new NotImplementedException();
        }
    }
}