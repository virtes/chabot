using Chabot.Messages;
using Chabot.Processing;
using Chabot.Routing.Implementation;
using Microsoft.Extensions.DependencyInjection;

namespace Chabot.Routing.Extensions
{
    public static class ProcessingPipelineBuilderExtensions
    {
        public static IProcessingPipelineBuilder<TMessage> UseRouting<TMessage>(
            this IProcessingPipelineBuilder<TMessage> pipelineBuilder)
            where TMessage : IMessage
        {
            pipelineBuilder.Services.AddScoped<RoutingMiddleware<TMessage>>();

            pipelineBuilder.UseMiddleware<RoutingMiddleware<TMessage>>();

            return pipelineBuilder;
        }
    }
}