using Chabot.Authentication.Implementation;
using Chabot.Authorization.Implementation;
using Chabot.Messages;

// ReSharper disable once CheckNamespace
namespace Chabot.Processing
{
    public static class ProcessingPipelineBuilderExtensions
    {
        //public static void RouteMessage<TMessage>(this IProcessingPipelineBuilder<TMessage> builder)
        //    where TMessage : IMessage
        //{
        //    builder.UseMiddleware<RouteMessageMiddleware<TMessage>>();
        //}

        public static void Authenticate<TMessage>(this IProcessingPipelineBuilder<TMessage> builder)
            where TMessage : IMessage
        {
            builder.UseMiddleware<AuthenticationMiddleware<TMessage>>();
        }

        public static void Authorize<TMessage>(this IProcessingPipelineBuilder<TMessage> builder)
            where TMessage : IMessage
        {
            builder.UseMiddleware<AuthorizationMiddleware<TMessage>>();
        }

        //public static void ExecuteCommand<TMessage>(this IProcessingPipelineBuilder<TMessage> builder)
        //    where TMessage : IMessage
        //{
        //    builder.UseMiddleware<CommandExecutionMiddleware<TMessage>>();
        //}
    }
}