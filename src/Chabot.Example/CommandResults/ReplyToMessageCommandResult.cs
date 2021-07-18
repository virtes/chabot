using System;
using System.Threading.Tasks;
using Chabot.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace Chabot.Example.CommandResults
{
    public class ReplyToMessageCommandResult : ICommandResult
    {
        private readonly string _sourceMessage;
        private readonly string _receiver;
        private readonly string _message;

        public ReplyToMessageCommandResult(string sourceMessage, string receiver, string message)
        {
            _sourceMessage = sourceMessage;
            _receiver = receiver;
            _message = message;
        }

        public ValueTask ExecuteResultAsync(IServiceProvider serviceProvider)
        {
            var messageService = serviceProvider.GetRequiredService<IMessageService>();

            messageService.SendMessage(_receiver, _message, _sourceMessage);

            return ValueTask.CompletedTask;
        }
    }
}