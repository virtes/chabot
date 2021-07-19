using System;
using System.Threading.Tasks;
using Chabot.Processing;
using Chabot.Processing.Implementation;

namespace Chabot.Example.Middlewares
{
    public class ExceptionHandlingMiddleware : IMiddleware<SimpleMessage>
    {
        public async ValueTask ExecuteAsync(IMessageContext<SimpleMessage> context, ProcessingDelegate<SimpleMessage> next)
        {
            try
            {
                await next(context);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unhandled exception while executing message {0} from {1} user ({2})",
                    context.Message.Id, context.User?.Id, context.Message.RawText);
                Console.WriteLine(e);
            }
        }
    }
}