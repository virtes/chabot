using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chabot.Commands;
using Chabot.Commands.Implementation;
using Chabot.Commands.Models;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Chabot.UnitTests.Commands
{
    public class CommandInfoBuilderTests
    {
        private class Metadata { }

        private class CommandGroup
        {
            public ICommandResult PureInvokeCommand()
                => default;

            public Task<ICommandResult> TaskInvokeCommand()
                => default;
        }

        private Mock<IMetadataContainerFactory> _metadataContainerFactoryMock;

        [SetUp]
        public void Setup()
        {
            _metadataContainerFactoryMock = new Mock<IMetadataContainerFactory>();
        }

        [Test]
        public void ShouldCollectMetadataFromAllCollectors()
        {
            var commandMethodInfo = typeof(CommandGroup).GetMethod(nameof(CommandGroup.PureInvokeCommand))!;

            var metadataFromFirstCollector = new Metadata();
            var metadataFromSecondCollector = new Metadata();

            var firstCollectorMock = new Mock<IMetadataCollector>();
            firstCollectorMock
                .Setup(c => c.Collect(commandMethodInfo))
                .Returns(new [] { metadataFromFirstCollector });
            var secondCollectorMock = new Mock<IMetadataCollector>();
            secondCollectorMock
                .Setup(c => c.Collect(commandMethodInfo))
                .Returns(new[] { metadataFromSecondCollector });

            var builder = CreateBuilder(new[]
            {
                firstCollectorMock.Object,
                secondCollectorMock.Object
            });

            builder.CreateFromCommandMethod(commandMethodInfo!);

            firstCollectorMock.VerifyAll();
            secondCollectorMock.VerifyAll();

            _metadataContainerFactoryMock
                .Verify(f => f.CreateContainer(It.Is<IReadOnlyCollection<object>>(m =>
                    (m.Count == 2)
                    && (m.First() == metadataFromFirstCollector)
                    && (m.Last() == metadataFromSecondCollector))));
        }

        [Test]
        public void ShouldCreateInfoWithPureInvokeType()
        {
            var commandMethodInfo = typeof(CommandGroup).GetMethod(nameof(CommandGroup.PureInvokeCommand))!;

            var builder = CreateBuilder();

            var commandInfo = builder.CreateFromCommandMethod(commandMethodInfo!);

            commandInfo.InvokeType.Should().Be(InvokeType.Pure);
        }

        [Test]
        public void ShouldCreateInfoWithTaskInvokeType()
        {
            var commandMethodInfo = typeof(CommandGroup).GetMethod(nameof(CommandGroup.TaskInvokeCommand))!;

            var builder = CreateBuilder();

            var commandInfo = builder.CreateFromCommandMethod(commandMethodInfo!);

            commandInfo.InvokeType.Should().Be(InvokeType.Task);
        }

        public CommandInfoBuilder CreateBuilder(IEnumerable<IMetadataCollector> metadataCollectors = null)
            => new CommandInfoBuilder(
                metadataCollectors: metadataCollectors ?? Array.Empty<IMetadataCollector>(),
                metadataContainerFactory: _metadataContainerFactoryMock.Object);
    }
}