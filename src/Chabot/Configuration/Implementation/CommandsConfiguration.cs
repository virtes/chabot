using System;

namespace Chabot.Configuration.Implementation
{
    public class CommandsConfiguration : ICommandsConfiguration
    {
        public CommandsConfiguration(Type[] commandGroupTypes)
        {
            CommandGroupTypes = commandGroupTypes;
        }

        public Type[] CommandGroupTypes { get; }
    }
}