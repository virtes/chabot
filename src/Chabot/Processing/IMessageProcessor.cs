using System.Threading.Tasks;

namespace Chabot.Processing
{
    public interface IMessageProcessor<in TMessage> where TMessage : IMessage
    {
        ValueTask ProcessAsync(TMessage message);
    }
}