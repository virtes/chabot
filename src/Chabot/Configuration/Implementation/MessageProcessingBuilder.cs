using System;
using System.Reflection;
using Chabot.Messages;
using Chabot.Processing;
using Chabot.Processing.Implementation;
using Microsoft.Extensions.DependencyInjection;

namespace Chabot.Configuration.Implementation
{
    public class MessageProcessingBuilder<TMessage> : IMessageProcessingBuilder<TMessage>
        where TMessage : IMessage
    {
        private readonly IChabotBuilder _chabotBuilder;
        private readonly IProcessingPipelineBuilder<TMessage> _processingPipelineBuilder;

        public MessageProcessingBuilder(IServiceCollection services,
            IChabotBuilder chabotBuilder)
        {
            _chabotBuilder = chabotBuilder;
            Services = services;

            _processingPipelineBuilder = new ProcessingPipelineBuilder<TMessage>(services);
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

        public void ScanCommandsFrom(Assembly assembly)
        {
            _chabotBuilder.ScanCommandsFrom(assembly, typeof(TMessage));
        }
    }
}