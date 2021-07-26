using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Chabot.Commands;
using Chabot.Messages;
using Chabot.Processing;
using Chabot.Processing.Implementation;
using Microsoft.Extensions.DependencyInjection;

namespace Chabot.Configuration.Implementation
{
    public class ChabotBuilder : IChabotBuilder
    {
        private readonly HashSet<Type> _commandGroupTypes;

        public ChabotBuilder(IServiceCollection services)
        {
            Services = services;
            _commandGroupTypes = new HashSet<Type>();
        }

        public IServiceCollection Services { get; }

        public void ScanCommandsFrom(Assembly assembly, Type messageType)
        {
            var commandGroupBaseOpenType = typeof(CommandGroupBase<>);
            var commandGroupBaseClosedType = commandGroupBaseOpenType.MakeGenericType(messageType);

            var commandGroupTypes = assembly
                .GetTypes()
                .Where(t => t.IsClass
                            && !t.IsAbstract
                            && commandGroupBaseClosedType.IsAssignableFrom(t));

            foreach (var commandGroupType in commandGroupTypes)
            {
                _commandGroupTypes.Add(commandGroupType);
            }
        }

        public IChabotBuilder ProcessMessage<TMessage>(
            Action<IMessageProcessingBuilder<TMessage>> configureBuilder)
            where TMessage : IMessage
        {
            var messageProcessingBuilder = new MessageProcessingBuilder<TMessage>(Services, this);

            configureBuilder(messageProcessingBuilder);

            messageProcessingBuilder.Build();

            RegisterServices<TMessage>();

            var commandsConfiguration = new CommandsConfiguration(_commandGroupTypes.ToArray());
            Services.AddSingleton<ICommandsConfiguration>(commandsConfiguration);

            return this;
        }

        public void Build()
        {
            RegisterCommandGroups();
        }

        private void RegisterServices<TMessage>()
            where TMessage : IMessage
        {
            Services.AddSingleton<IMessageProcessor<TMessage>, MessageProcessor<TMessage>>();
            Services.AddSingleton<IMessageContextFactory<TMessage>, MessageContextFactory<TMessage>>();
        }

        private void RegisterCommandGroups()
        {
            foreach (var commandGroupType in _commandGroupTypes)
            {
                Services.AddScoped(commandGroupType);
            }
        }
    }
}