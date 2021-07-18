using System;
using Chabot.Messages;
using Microsoft.Extensions.DependencyInjection;

namespace Chabot.Configuration
{
    public interface IChabotBuilder
    {
        IServiceCollection Services { get; }

        IChabotBuilder ProcessMessage<TMessage>(Action<IMessageProcessingBuilder<TMessage>> configureBuilder)
            where TMessage : IMessage;
    }
}