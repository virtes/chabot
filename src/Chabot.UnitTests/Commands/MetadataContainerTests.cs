using System.Collections.Generic;
using Chabot.Commands.Implementation;
using FluentAssertions;
using NUnit.Framework;

namespace Chabot.UnitTests.Commands
{
    public class MetadataContainerTests
    {
        private class FirstMetadata { }
        private class SecondMetadata { }

        [Test]
        public void ShouldProvideMetadataOfTypeInOrder()
        {
            var expectedMeta1 = new FirstMetadata();
            var expectedMeta2 = new FirstMetadata();
            var extraMeta = new SecondMetadata();

            var allMetadata = new List<object>
            {
                expectedMeta1,
                extraMeta,
                extraMeta,
                expectedMeta2,
                extraMeta
            };

            var container = new MetadataContainer(allMetadata);

            var actualMetadata = container.GetAllOfType<FirstMetadata>();
            actualMetadata.Length.Should().Be(2);
            actualMetadata[0].Should().Be(expectedMeta1);
            actualMetadata[1].Should().Be(expectedMeta2);
        }
    }
}