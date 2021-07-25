using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Chabot.Commands.Models;

namespace Chabot.Commands.Implementation
{
    public class CommandInfoBuilder : ICommandInfoFactory
    {
        private readonly IEnumerable<IMetadataCollector> _metadataCollectors;
        private readonly IMetadataContainerFactory _metadataContainerFactory;

        public CommandInfoBuilder(IEnumerable<IMetadataCollector> metadataCollectors,
            IMetadataContainerFactory metadataContainerFactory)
        {
            _metadataCollectors = metadataCollectors;
            _metadataContainerFactory = metadataContainerFactory;
        }

        public CommandInfo CreateFromCommandMethod(MethodInfo methodInfo)
        {
            var invokeType = GetInvokeType(methodInfo);
            var metadata = CollectMetadata(methodInfo);

            return new CommandInfo(
                methodInfo: methodInfo,
                invokeType: invokeType,
                metadata: metadata);
        }

        private static InvokeType GetInvokeType(MethodInfo methodInfo)
        {
            var returnType = methodInfo.ReturnType;

            bool IsTask()
                => (returnType != null)
                   && returnType.IsGenericType
                   && (returnType.GetGenericTypeDefinition() == typeof(Task<>));

            return IsTask()
                ? InvokeType.Task
                : InvokeType.Pure;
        }

        private IMetadataContainer CollectMetadata(MethodInfo methodInfo)
        {
            var allMetadata = new List<object>();

            foreach (var metadataCollector in _metadataCollectors)
            {
                allMetadata.AddRange(metadataCollector.Collect(methodInfo));
            }

            return _metadataContainerFactory.CreateContainer(allMetadata);
        }
    }
}