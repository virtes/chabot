using System;
using System.Threading.Tasks;
using Chabot.Processing;
using Chabot.Processing.Implementation;

namespace Chabot.UnitTests.Fakes
{
    public class Middleware : IMiddleware<Message>
    {
        private readonly Func<Task> _action;

        public Middleware(Func<Task> action)
        {
            _action = action;
        }

        public async ValueTask ExecuteAsync(IMessageContext<Message> context, ProcessingDelegate<Message> next)
        {
            await _action();

            await next(context);
        }
    }
}