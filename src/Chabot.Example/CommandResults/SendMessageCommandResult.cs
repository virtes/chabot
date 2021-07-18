using System;
using System.Threading.Tasks;
using Chabot.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace Chabot.Example.CommandResults
{
    public class SendMessageCommandResult : ICommandResult
    {
        private readonly string _receiver;
        private readonly string _text;

        public SendMessageCommandResult(string receiver, string text)
        {
            _receiver = receiver;
            _text = text;
        }

        public ValueTask ExecuteResultAsync(IServiceProvider serviceProvider)
        {
            var messageService = serviceProvider.GetRequiredService<IMessageService>();

            messageService.SendMessage(_receiver, _text);

            return ValueTask.CompletedTask;
        }
    }
}