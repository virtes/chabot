using System.Reflection;
using Chabot.Commands.Models;

// ReSharper disable once CheckNamespace
namespace Chabot.Commands
{
    public class CommandInfo
    {
        public CommandInfo(MethodInfo methodInfo, InvokeType invokeType, IMetadataContainer metadata)
        {
            MethodInfo = methodInfo;
            InvokeType = invokeType;
            Metadata = metadata;
        }

        public MethodInfo MethodInfo { get; }

        public InvokeType InvokeType { get; }

        public IMetadataContainer Metadata { get; }
    }
}