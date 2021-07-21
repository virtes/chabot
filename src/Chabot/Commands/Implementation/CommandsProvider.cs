using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Chabot.Configuration;

namespace Chabot.Commands.Implementation
{
    public class CommandsProvider : ICommandsProvider
    {
        private readonly ICommandsConfiguration _commandsConfiguration;
        private readonly ICommandInfoFactory _commandInfoFactory;

        private readonly ConcurrentDictionary<Type, IReadOnlyList<CommandInfo>> _commandsByMessageType
            = new ConcurrentDictionary<Type, IReadOnlyList<CommandInfo>>();

        public CommandsProvider(ICommandsConfiguration commandsConfiguration,
            ICommandInfoFactory commandInfoFactory)
        {
            _commandsConfiguration = commandsConfiguration;
            _commandInfoFactory = commandInfoFactory;
        }

        public IReadOnlyList<CommandInfo> GetCommandsByMessageType(Type messageType)
        {
            return _commandsByMessageType.GetOrAdd(messageType, ScanCommands);
        }

        private List<CommandInfo> ScanCommands(Type messageType)
        {
            var result = new List<CommandInfo>();

            foreach (var commandGroupType in _commandsConfiguration.CommandGroupTypes)
            {
                result.AddRange(ScanCommandsInCommandsGroup(commandGroupType));
            }

            return result;
        }

        private List<CommandInfo> ScanCommandsInCommandsGroup(Type commandGroupType)
        {
            static bool IsPublic(MethodInfo methodInfo)
                => methodInfo.IsPublic;

            static bool IsValidCommandResultType(Type type)
                => typeof(ICommandResult).IsAssignableFrom(type);

            static bool IsValidAsyncCommandResultType(Type type)
                => type.IsGenericType
                   && type.GetGenericTypeDefinition() == typeof(Task<>)
                   && IsValidCommandResultType(type.GetGenericArguments()[0]);

            var commandMethods = commandGroupType
                .GetMethods()
                .Where(mi => IsPublic(mi)
                             && (IsValidCommandResultType(mi.ReturnType) || IsValidAsyncCommandResultType(mi.ReturnType)))
                .ToList();

            return commandMethods.ConvertAll(mi => _commandInfoFactory.CreateFromCommandMethod(mi));
        }
    }
}