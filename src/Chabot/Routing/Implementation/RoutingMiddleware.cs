using System.Threading.Tasks;
using Chabot.Messages;
using Chabot.Processing;
using Chabot.Processing.Implementation;

namespace Chabot.Routing.Implementation
{
    public class RoutingMiddleware<TMessage> : IMiddleware<TMessage>
        where TMessage : IMessage
    {
        private readonly IMessageRouter<TMessage> _messageRouter;

        public RoutingMiddleware(IMessageRouter<TMessage> messageRouter)
        {
            _messageRouter = messageRouter;
        }

        public ValueTask ExecuteAsync(IMessageContext<TMessage> context, ProcessingDelegate<TMessage> next)
        {
            var commandInfo = _messageRouter.RouteMessage(context.Message);

            context.Command = commandInfo;

            return next(context);
        }
    }
}