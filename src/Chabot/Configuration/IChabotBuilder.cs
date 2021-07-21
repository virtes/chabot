using System;
using System.Reflection;
using Chabot.Messages;
using Microsoft.Extensions.DependencyInjection;

namespace Chabot.Configuration
{
    public interface IChabotBuilder
    {
        IServiceCollection Services { get; }

        void ScanCommandsFrom(Assembly assembly, Type messageType);

        IChabotBuilder ProcessMessage<TMessage>(Action<IMessageProcessingBuilder<TMessage>> configureBuilder)
            where TMessage : IMessage;
    }
}