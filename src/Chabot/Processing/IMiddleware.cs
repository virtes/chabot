using System.Threading.Tasks;
using Chabot.Processing.Implementation;

namespace Chabot.Processing
{
    public interface IMiddleware<TMessage> where TMessage : IMessage
    {
        ValueTask ExecuteAsync(MessageContext<TMessage> context, ProcessingDelegate<TMessage> next);
    }
}