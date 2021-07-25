using System.Collections.Generic;

namespace Chabot.Commands.Implementation
{
    public class MetadataContainerFactory : IMetadataContainerFactory
    {
        public IMetadataContainer CreateContainer(IReadOnlyCollection<object> allMetadata)
        {
            return new MetadataContainer(allMetadata);
        }
    }
}