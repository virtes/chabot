using System;
using Chabot.Commands;
using Chabot.Commands.Exceptions;
using Chabot.Commands.Implementation;
using Chabot.Commands.Models;
using FluentAssertions;
using NUnit.Framework;

namespace Chabot.UnitTests.Commands
{
    public class MetadataFromAttributesCollectorTests
    {
        private class Metadata
        {
            public string Value { get; set; }
        }

        private class SomeAttribute : Attribute { }

        [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
        private class MetadataBinderAttribute : Attribute, IMetadataBinder<Metadata>
        {
            public MetadataBinderAttribute(string value, MetadataInheritanceType inheritanceType)
            {
                Metadata = new Metadata
                {
                    Value = value
                };
                InheritanceType = inheritanceType;
            }

            public Metadata Metadata { get; }

            public MetadataInheritanceType InheritanceType { get; }
        }

        [MetadataBinder("class-value-1", MetadataInheritanceType.Override)]
        [MetadataBinder("class-value-2", MetadataInheritanceType.Override)]
        [SomeAttribute]
        private class ClassWithOverrideMetadata
        {
            [MetadataBinder("method-value-1", MetadataInheritanceType.Override)]
            [MetadataBinder("method-value-2", MetadataInheritanceType.Override)]
            [SomeAttribute]
            public void Method() { }
        }

        [MetadataBinder("class-value-1", MetadataInheritanceType.Append)]
        [MetadataBinder("class-value-2", MetadataInheritanceType.Append)]
        [SomeAttribute]
        private class ClassWithAppendMetadata
        {
            [MetadataBinder("method-value-1", MetadataInheritanceType.Append)]
            [MetadataBinder("method-value-2", MetadataInheritanceType.Append)]
            [SomeAttribute]
            public void Method() { }
        }

        [MetadataBinder("class-value", MetadataInheritanceType.Append)]
        private class ClassWithInvalidInheritanceMetadataBinding
        {
            [MetadataBinder("method-value", MetadataInheritanceType.Override)]
            public void Method() { }
        }

        [Test]
        public void ShouldCollectMetadataWithInheritanceOverridingFromMethod()
        {
            var collector = CreateCollector();

            var allMetadata = collector.Collect(typeof(ClassWithOverrideMetadata).GetMethod(nameof(ClassWithOverrideMetadata.Method))!);
            allMetadata.Count.Should().Be(2);
            allMetadata.Should().AllBeOfType<Metadata>();

            ((Metadata)allMetadata[0]).Value.Should().Be("method-value-1");
            ((Metadata)allMetadata[1]).Value.Should().Be("method-value-2");
        }

        [Test]
        public void ShouldCollectMetadataWithInheritanceAppending()
        {
            var collector = CreateCollector();

            var allMetadata = collector.Collect(typeof(ClassWithAppendMetadata).GetMethod(nameof(ClassWithAppendMetadata.Method))!);
            allMetadata.Count.Should().Be(4);
            allMetadata.Should().AllBeOfType<Metadata>();

            ((Metadata) allMetadata[0]).Value.Should().Be("method-value-1");
            ((Metadata) allMetadata[1]).Value.Should().Be("method-value-2");
            ((Metadata) allMetadata[2]).Value.Should().Be("class-value-1");
            ((Metadata) allMetadata[3]).Value.Should().Be("class-value-2");
        }

        [Test]
        public void ShouldThrowWhenSeveralInheritanceTypeInForTheSameMetadataIsDefined()
        {
            var collector = CreateCollector();

            collector.Invoking(c =>
                c.Collect(typeof(ClassWithInvalidInheritanceMetadataBinding).GetMethod(
                    nameof(ClassWithInvalidInheritanceMetadataBinding.Method))!))
                .Should().Throw<InvalidMetadataInheritanceTypeException>();
        }

        private MetadataFromAttributesCollector CreateCollector()
            => new MetadataFromAttributesCollector();
    }
}