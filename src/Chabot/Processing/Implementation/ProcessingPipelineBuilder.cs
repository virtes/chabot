using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chabot.Messages;
using Chabot.Processing.Exceptions;
using Microsoft.Extensions.DependencyInjection;

namespace Chabot.Processing.Implementation
{
    public class ProcessingPipelineBuilder<TMessage> : IProcessingPipelineBuilder<TMessage> where TMessage : IMessage
    {
        private readonly List<Func<ProcessingDelegate<TMessage>, ProcessingDelegate<TMessage>>> _middlewares;

        public ProcessingPipelineBuilder(IServiceCollection services)
        {
            _middlewares = new List<Func<ProcessingDelegate<TMessage>, ProcessingDelegate<TMessage>>>();

            Services = services;
        }

        public IServiceCollection Services { get; }

        public bool ThrowWhenPipelineReachedTheEnd { get; set; } = true;

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

                    var executeTask = middleware.ExecuteAsync(context, next);
                    if (!executeTask.IsCompletedSuccessfully)
                        await executeTask;
                };
            });
        }

        public ProcessingDelegate<TMessage> BuildProcessingEntryPoint()
        {
            ProcessingDelegate<TMessage> entryPoint;

            if (ThrowWhenPipelineReachedTheEnd)
            {
                entryPoint = _ => throw new MessageProcessorReachedTheEndException(typeof(TMessage));
            }
            else
            {
                entryPoint = _ => ValueTask.CompletedTask;
            }

            foreach (var middleware in _middlewares.AsEnumerable().Reverse())
            {
                entryPoint = middleware(entryPoint);
            }

            return entryPoint;
        }
    }
}