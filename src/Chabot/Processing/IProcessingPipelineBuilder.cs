using System;
using Chabot.Messages;
using Chabot.Processing.Implementation;
using Microsoft.Extensions.DependencyInjection;

namespace Chabot.Processing
{
    public interface IProcessingPipelineBuilder<TMessage> where TMessage : IMessage
    {
        IServiceCollection Services { get; }

        bool ThrowWhenPipelineReachedTheEnd { get; set; }

        IProcessingPipelineBuilder<TMessage> Use(Func<ProcessingDelegate<TMessage>, ProcessingDelegate<TMessage>> middleware);

        IProcessingPipelineBuilder<TMessage> UseMiddleware<T>()
            where T : IMiddleware<TMessage>;

        ProcessingDelegate<TMessage> BuildProcessingEntryPoint();
    }
}