using System;
using Chabot.Messages;
using Chabot.Processing;
using Chabot.Processing.Implementation;
using Microsoft.Extensions.DependencyInjection;

namespace Chabot.Configuration.Implementation
{
    public class MessageProcessingBuilder<TMessage> : IMessageProcessingBuilder<TMessage>
        where TMessage : IMessage
    {
        private readonly IProcessingPipelineBuilder<TMessage> _processingPipelineBuilder;

        public MessageProcessingBuilder(IServiceCollection services)
        {
            Services = services;

            _processingPipelineBuilder = new ProcessingPipelineBuilder<TMessage>();
        }

        public IServiceCollection Services { get; }

        public IMessageProcessingBuilder<TMessage> ConfigurePipeline(
            Action<IProcessingPipelineBuilder<TMessage>> configureBuilder)
        {
            configureBuilder(_processingPipelineBuilder);

            return this;
        }

        public void Build()
        {
            var processingEntryPoint = _processingPipelineBuilder.BuildProcessingEntryPoint();

            var configuration = new MessageProcessingConfiguration<TMessage>(processingEntryPoint);

            Services.AddSingleton<IMessageProcessingConfiguration<TMessage>>(configuration);
        }
    }
}