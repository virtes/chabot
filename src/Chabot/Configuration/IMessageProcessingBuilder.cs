using System;
using Chabot.Messages;
using Chabot.Processing;
using Microsoft.Extensions.DependencyInjection;

namespace Chabot.Configuration
{
    public interface IMessageProcessingBuilder<TMessage> where TMessage : IMessage
    {
        IServiceCollection Services { get; }

        IMessageProcessingBuilder<TMessage> ConfigurePipeline(
            Action<IProcessingPipelineBuilder<TMessage>> configureBuilder);
    }
}