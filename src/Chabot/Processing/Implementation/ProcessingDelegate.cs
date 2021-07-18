using System.Threading.Tasks;

namespace Chabot.Processing.Implementation
{
    public delegate ValueTask ProcessingDelegate<TMessage>(MessageContext<TMessage> context)
        where TMessage : IMessage;
}