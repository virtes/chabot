using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Chabot.Commands.Implementation
{
    public class MetadataContainer : IMetadataContainer
    {
        private readonly IReadOnlyCollection<object> _allMetadata;

        private readonly ConcurrentDictionary<Type, object> _metadataArraysByType
            = new ConcurrentDictionary<Type, object>();

        public MetadataContainer(IReadOnlyCollection<object> allMetadata)
        {
            _allMetadata = allMetadata;
        }

        public T[] GetAllOfType<T>() where T : class
        {
            return (T[])_metadataArraysByType.GetOrAdd(typeof(T), _ => CollectArrayOfMetadata<T>());
        }

        private object CollectArrayOfMetadata<T>()
        {
            return _allMetadata
                .Where(m => m.GetType() == typeof(T))
                .Cast<T>()
                .ToArray();
        }
    }
}