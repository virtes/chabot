using Chabot.Commands.Implementation;
using FluentAssertions;
using NUnit.Framework;

namespace Chabot.UnitTests.Commands
{
    public class MetadataContainerFactoryTests
    {
        private class Metadata { }

        [Test]
        public void ShouldCreateMetadataContainer()
        {
            var factory = CreateFactory();

            var allMetadata = new object[]
            {
                new Metadata()
            };

            var actualContainer = factory.CreateContainer(allMetadata);

            actualContainer.Should().BeOfType<MetadataContainer>();
        }

        private MetadataContainerFactory CreateFactory()
            => new MetadataContainerFactory();
    }
}