using System;
using System.Threading.Tasks;
using Chabot.Messages;
using Chabot.Processing;
using Chabot.Processing.Implementation;

namespace Chabot.Authentication.Implementation
{
    public class AuthenticationMiddleware<TMessage> : IMiddleware<TMessage>
        where TMessage : IMessage
    {
        public ValueTask ExecuteAsync(IMessageContext<TMessage> context, ProcessingDelegate<TMessage> next)
        {
            throw new NotImplementedException();
        }
    }
}