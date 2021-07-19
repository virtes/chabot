//using Chabot.Commands;
//using Chabot.Example.CommandResults;

//namespace Chabot.Example.Commands
//{
//    /// <summary>
//    /// Base class for processing commands on 'SimpleMessage' messages
//    /// </summary>
//    public abstract class CommandGroup : CommandGroupBase<SimpleMessage>
//    {
//        protected ICommandResult SendMessage(string text)
//        {
//            return new SendMessageCommandResult(Context.User.Id, text);
//        }

//        protected ICommandResult Reply(string text)
//        {
//            return new ReplyToMessageCommandResult(Context.Message.RawText, Context.User.Id, text);
//        }
//    }
//}