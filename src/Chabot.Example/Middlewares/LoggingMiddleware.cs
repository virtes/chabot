using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Chabot.Processing;
using Chabot.Processing.Implementation;

namespace Chabot.Example.Middlewares
{
    public class LoggingMiddleware : IMiddleware<SimpleMessage>
    {
        public async ValueTask ExecuteAsync(IMessageContext<SimpleMessage> context, ProcessingDelegate<SimpleMessage> next)
        {
            Console.WriteLine($"-- Received a new message: {context.Message.RawText} --");
            var stopwatch = Stopwatch.StartNew();

            await next(context);

            Console.WriteLine("-- Message processed in {0} --", stopwatch.Elapsed);
        }
    }
}