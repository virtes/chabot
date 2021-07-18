using System;
using Chabot.Messages;
using Chabot.Processing;
using Chabot.Processing.Implementation;
using Microsoft.Extensions.DependencyInjection;

namespace Chabot.Configuration.Implementation
{
    public class ChabotBuilder : IChabotBuilder
    {
        public ChabotBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; }

        public IChabotBuilder ProcessMessage<TMessage>(
            Action<IMessageProcessingBuilder<TMessage>> configureBuilder)
            where TMessage : IMessage
        {
            var messageProcessingBuilder = new MessageProcessingBuilder<TMessage>(Services);

            configureBuilder(messageProcessingBuilder);

            messageProcessingBuilder.Build();

            AddServices<TMessage>();

            return this;
        }

        private void AddServices<TMessage>()
            where TMessage : IMessage
        {
            Services.AddSingleton<IMessageProcessor<TMessage>, MessageProcessor<TMessage>>();
        }
    }
}