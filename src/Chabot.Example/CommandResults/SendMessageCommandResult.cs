//using System;
//using System.Threading.Tasks;
//using Chabot.Commands;
//using Microsoft.Extensions.DependencyInjection;

//namespace Chabot.Example.CommandResults
//{
//    public class SendMessageCommandResult : ICommandResult
//    {
//        private readonly string _userId;
//        private readonly string _text;

//        public SendMessageCommandResult(string userId, string text)
//        {
//            _userId = userId;
//            _text = text;
//        }

//        public ValueTask ExecuteResultAsync(IServiceProvider serviceProvider)
//        {
//            var messageService = serviceProvider.GetRequiredService<IInteractionWithUser>();

//            messageService.SendMessage(_userId, _text, null);

//            return ValueTask.CompletedTask;
//        }
//    }
//}