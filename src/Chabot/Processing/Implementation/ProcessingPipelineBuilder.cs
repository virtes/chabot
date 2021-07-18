using System;
using System.Collections.Generic;
using System.Linq;
using Chabot.Processing.Exceptions;
using Microsoft.Extensions.DependencyInjection;

namespace Chabot.Processing.Implementation
{
    public class ProcessingPipelineBuilder<TMessage> : IProcessingPipelineBuilder<TMessage> where TMessage : IMessage
    {
        private readonly List<Func<ProcessingDelegate<TMessage>, ProcessingDelegate<TMessage>>> _middlewares;

        public ProcessingPipelineBuilder()
        {
            _middlewares = new List<Func<ProcessingDelegate<TMessage>, ProcessingDelegate<TMessage>>>();
        }

        public IProcessingPipelineBuilder<TMessage> Use(Func<ProcessingDelegate<TMessage>, ProcessingDelegate<TMessage>> middleware)
        {
            _middlewares.Add(middleware);

            return this;
        }

        public IProcessingPipelineBuilder<TMessage> UseMiddleware<T>()
            where T : IMiddleware<TMessage>
        {
            return Use(next =>
            {
                return async context =>
                {
                    var middleware = context.Services.GetRequiredService<T>();

                    await middleware.ExecuteAsync(context, next);
                };
            });
        }

        public ProcessingDelegate<TMessage> BuildProcessingEntryPoint()
        {
            ProcessingDelegate<TMessage> entryPoint = context
                => throw new MessageProcessorReachedTheEndException(typeof(TMessage));

            foreach (var middleware in _middlewares.AsEnumerable().Reverse())
            {
                entryPoint = middleware(entryPoint);
            }

            return entryPoint;
        }
    }
}