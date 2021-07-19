using System.Threading.Tasks;
using Chabot.Messages;
using Chabot.Processing.Implementation;

namespace Chabot.Processing
{
    public interface IMiddleware<TMessage> where TMessage : IMessage
    {
        ValueTask ExecuteAsync(IMessageContext<TMessage> context, ProcessingDelegate<TMessage> next);
    }
}