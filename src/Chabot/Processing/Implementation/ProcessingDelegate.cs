using System.Threading.Tasks;
using Chabot.Messages;

namespace Chabot.Processing.Implementation
{
    public delegate ValueTask ProcessingDelegate<TMessage>(MessageContext<TMessage> context)
        where TMessage : IMessage;
}