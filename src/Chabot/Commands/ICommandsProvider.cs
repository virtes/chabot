using System;
using System.Collections.Generic;

namespace Chabot.Commands
{
    public interface ICommandsProvider
    {
        IReadOnlyList<CommandInfo> GetCommandsByMessageType(Type messageType);
    }
}