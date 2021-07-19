using System.Threading.Tasks;
using Chabot.Messages;

namespace Chabot.Processing
{
    public interface IMessageProcessor<in TMessage> where TMessage : IMessage
    {
        ValueTask ProcessAsync(TMessage message);
    }
}