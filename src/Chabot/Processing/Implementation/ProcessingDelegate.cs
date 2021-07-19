using System.Threading.Tasks;
using Chabot.Messages;

namespace Chabot.Processing.Implementation
{
    public delegate ValueTask ProcessingDelegate<in TMessage>(IMessageContext<TMessage> context)
        where TMessage : IMessage;
}