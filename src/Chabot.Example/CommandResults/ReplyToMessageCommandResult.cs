//using System;
//using System.Threading.Tasks;
//using Chabot.Commands;
//using Microsoft.Extensions.DependencyInjection;

//namespace Chabot.Example.CommandResults
//{
//    public class ReplyToMessageCommandResult : ICommandResult
//    {
//        private readonly string _sourceMessage;
//        private readonly string _userId;
//        private readonly string _message;

//        public ReplyToMessageCommandResult(string sourceMessage, string userId, string message)
//        {
//            _sourceMessage = sourceMessage;
//            _userId = userId;
//            _message = message;
//        }

//        public ValueTask ExecuteResultAsync(IServiceProvider serviceProvider)
//        {
//            var messageService = serviceProvider.GetRequiredService<IInteractionWithUser>();

//            messageService.SendMessage(_userId, _message, _sourceMessage);

//            return ValueTask.CompletedTask;
//        }
//    }
//}