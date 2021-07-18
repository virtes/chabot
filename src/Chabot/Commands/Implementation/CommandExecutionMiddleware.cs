using System;
using System.Threading.Tasks;
using Chabot.Processing;
using Chabot.Processing.Implementation;

namespace Chabot.Commands.Implementation
{
    public class CommandExecutionMiddleware<TMessage> : IMiddleware<TMessage>
        where TMessage : IMessage
    {
        public ValueTask ExecuteAsync(MessageContext<TMessage> context, ProcessingDelegate<TMessage> next)
        {
            throw new NotImplementedException();
        }
    }
}