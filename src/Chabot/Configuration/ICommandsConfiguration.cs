using System;

namespace Chabot.Configuration
{
    public interface ICommandsConfiguration
    {
        Type[] CommandGroupTypes { get; }
    }
}