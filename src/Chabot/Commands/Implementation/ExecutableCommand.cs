using System;
using System.Threading.Tasks;
using Chabot.Messages;
using Chabot.Processing;

namespace Chabot.Commands.Implementation
{
    public class ExecutableCommand<TMessage> : IExecutableCommand<TMessage>
        where TMessage : IMessage
    {
        private readonly Func<IMessageContext<TMessage>, ValueTask> _executeFunc;

        public ExecutableCommand(CommandInfo commandInfo, Func<IMessageContext<TMessage>, ValueTask> executeFunc)
        {
            _executeFunc = executeFunc;
            CommandInfo = commandInfo;
        }

        public CommandInfo CommandInfo { get; }

        public async ValueTask ExecuteAsync(IMessageContext<TMessage> context)
        {
            var executeTask = _executeFunc(context);
            if (!executeTask.IsCompletedSuccessfully)
            {
                await executeTask;
            }
        }
    }
}