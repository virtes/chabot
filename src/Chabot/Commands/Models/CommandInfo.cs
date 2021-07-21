using System.Reflection;

// ReSharper disable once CheckNamespace
namespace Chabot.Commands
{
    public class CommandInfo
    {
        public CommandInfo(MethodInfo methodInfo, bool isAsync, ICommandMetadata metadata)
        {
            MethodInfo = methodInfo;
            IsAsync = isAsync;
            Metadata = metadata;
        }

        public MethodInfo MethodInfo { get; }

        public bool IsAsync { get; }

        public ICommandMetadata Metadata { get; }
    }
}