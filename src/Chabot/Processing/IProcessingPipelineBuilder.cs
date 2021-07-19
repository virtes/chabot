using System;
using Chabot.Messages;
using Chabot.Processing.Implementation;

namespace Chabot.Processing
{
    public interface IProcessingPipelineBuilder<TMessage> where TMessage : IMessage
    {
        IProcessingPipelineBuilder<TMessage> Use(Func<ProcessingDelegate<TMessage>, ProcessingDelegate<TMessage>> middleware);

        IProcessingPipelineBuilder<TMessage> UseMiddleware<T>()
            where T : IMiddleware<TMessage>;

        ProcessingDelegate<TMessage> BuildProcessingEntryPoint();
    }
}