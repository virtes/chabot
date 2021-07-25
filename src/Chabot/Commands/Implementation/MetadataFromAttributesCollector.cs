using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Chabot.Commands.Exceptions;
using Chabot.Commands.Models;

namespace Chabot.Commands.Implementation
{
    public class MetadataFromAttributesCollector : IMetadataCollector
    {
        public IReadOnlyList<object> Collect(MethodInfo methodInfo)
        {
            var allMetadata = new List<MetadataBinding>();

            allMetadata.AddRange(CollectMetadataBindingsFromType(methodInfo.DeclaringType!));
            allMetadata.AddRange(CollectMetadataBindingsFromMethod(methodInfo));

            return allMetadata
                .GroupBy(m => m.Metadata.GetType())
                .SelectMany(g =>
                {
                    var bindings = g.ToList();
                    var inheritanceType = GetInheritanceType(bindings);

                    return inheritanceType switch
                    {
                        MetadataInheritanceType.Override => CollectMetadataFromOverrideBindings(bindings),
                        MetadataInheritanceType.Append => CollectMetadataFromAppendBindings(bindings),
                        _ => throw new ArgumentOutOfRangeException(nameof(inheritanceType), inheritanceType, null)
                    };
                })
                .ToList();
        }

        private static List<MetadataBinding> CollectMetadataBindingsFromType(Type type)
        {
            var attributes = type.GetCustomAttributes();

            return CollectFromAttributes(attributes, 10);
        }

        private static List<MetadataBinding> CollectMetadataBindingsFromMethod(MethodInfo methodInfo)
        {
            var attributes = methodInfo.GetCustomAttributes();

            return CollectFromAttributes(attributes, 5);
        }

        private static List<MetadataBinding> CollectFromAttributes(IEnumerable<Attribute> attributes, int order)
        {
            var metadataBinderOpenType = typeof(IMetadataBinder<>);

            var metadataBinderAttributes = attributes
                .Where(a => a.GetType()
                    .GetInterfaces()
                    .Any(i => i.IsGenericType
                                && i.GetGenericTypeDefinition() is var interfaceGenericType
                                && interfaceGenericType == metadataBinderOpenType))
                .ToList();

            var result = new List<MetadataBinding>();

            foreach (var metadataBinderAttribute in metadataBinderAttributes)
            {
                var metadataProperty = metadataBinderAttribute.GetType()
                    .GetProperty(nameof(IMetadataBinder<object>.Metadata))!;
                var inheritanceTypeProperty = metadataBinderAttribute.GetType()
                    .GetProperty(nameof(IMetadataBinder<object>.InheritanceType))!;

                var metadata = metadataProperty.GetValue(metadataBinderAttribute)!;
                var inheritanceType = (MetadataInheritanceType)inheritanceTypeProperty.GetValue(metadataBinderAttribute)!;

                result.Add(new MetadataBinding(metadata, inheritanceType, order));
            }

            return result;
        }

        private static List<object> CollectMetadataFromOverrideBindings(List<MetadataBinding> bindings)
        {
            var minOrder = bindings.Min(b => b.Order);

            return bindings
                .Where(b => b.Order == minOrder)
                .Select(b => b.Metadata)
                .ToList();
        }

        private static List<object> CollectMetadataFromAppendBindings(List<MetadataBinding> bindings)
            => bindings
                .OrderBy(b => b.Order)
                .Select(b => b.Metadata)
                .ToList();

        private static MetadataInheritanceType GetInheritanceType(IReadOnlyList<MetadataBinding> bindings)
        {
            var firstInheritanceType = bindings.First().InheritanceType;

            foreach (var binding in bindings)
            {
                if (binding.InheritanceType != firstInheritanceType)
                {
                    throw new InvalidMetadataInheritanceTypeException(binding.Metadata.GetType());
                }
            }

            return firstInheritanceType;
        }

        private readonly struct MetadataBinding
        {
            public MetadataBinding(object metadata, MetadataInheritanceType inheritanceType, int order)
            {
                Metadata = metadata;
                InheritanceType = inheritanceType;
                Order = order;
            }

            public object Metadata { get; }

            public MetadataInheritanceType InheritanceType { get; }

            public int Order { get; }
        }
    }
}