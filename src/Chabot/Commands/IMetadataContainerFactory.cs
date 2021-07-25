using System.Collections.Generic;

namespace Chabot.Commands
{
    public interface IMetadataContainerFactory
    {
        IMetadataContainer CreateContainer(IReadOnlyCollection<object> allMetadata);
    }
}