using Chabot.Commands.Models;

namespace Chabot.Commands
{
    public interface IMetadataBinder<out TMetadata>
    {
        public TMetadata Metadata { get; }

        public MetadataInheritanceType InheritanceType { get; }
    }
}