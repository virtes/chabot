using System.Threading.Tasks;
using Chabot.Messages;
using Chabot.Processing;

namespace Chabot.Commands
{
    public interface IExecutableCommand<in TMessage>
        where TMessage : IMessage
    {
        public CommandInfo CommandInfo { get; }

        ValueTask ExecuteAsync(IMessageContext<TMessage> context);
    }
}