using System;
using System.Reflection;
using System.Threading.Tasks;
using Chabot.Commands;
using Chabot.Commands.Implementation;
using Chabot.Configuration.Implementation;
using Chabot.UnitTests.Fakes;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Chabot.UnitTests.Commands
{
    public class CommandsProviderTests
    {
        private Mock<ICommandInfoFactory> _commandInfoFactoryMock;

        [SetUp]
        public void Setup()
        {
            _commandInfoFactoryMock = new Mock<ICommandInfoFactory>();
            _commandInfoFactoryMock
                .Setup(f => f.CreateFromCommandMethod(It.IsAny<MethodInfo>()))
                .Returns<MethodInfo>(mi => new CommandInfo(mi, default, default!));
        }

        public class SyncCommandGroup : CommandGroupBase<Message>
        {
            public class InheritedCommandResult : ICommandResult { }

            private ICommandResult PrivateMethod()
                => default!;

            public int InvalidReturnType()
                => default;

            public ICommandResult ValidCommand()
                => default!;

            public InheritedCommandResult ValidCommand_2()
                => default!;
        }

        public class AsyncCommandGroup : CommandGroupBase<Message>
        {
            public class InheritedCommandResult : ICommandResult { }

            private Task<ICommandResult> PrivateMethod()
                => default!;

            public Task<int> InvalidReturnType()
                => default;

            public Task<ICommandResult> ValidCommand()
                => default!;

            public Task<InheritedCommandResult> ValidCommand_2()
                => default!;
        }

        [Test]
        public void ShouldScanCommandsFromSyncCommandGroup()
        {
            var provider = CreateProvider(typeof(SyncCommandGroup));

            var commands = provider.GetCommandsByMessageType(typeof(Message));

            commands.Count.Should().Be(2);
            commands.Should().Contain(ci => ci.MethodInfo == typeof(SyncCommandGroup).GetMethod(nameof(SyncCommandGroup.ValidCommand)));
            commands.Should().Contain(ci => ci.MethodInfo == typeof(SyncCommandGroup).GetMethod(nameof(SyncCommandGroup.ValidCommand_2)));
        }

        [Test]
        public void ShouldScanCommandsFromAsyncCommandGroup()
        {
            var provider = CreateProvider(typeof(AsyncCommandGroup));

            var commands = provider.GetCommandsByMessageType(typeof(Message));

            commands.Count.Should().Be(2);
            commands.Should().Contain(ci => ci.MethodInfo == typeof(AsyncCommandGroup).GetMethod(nameof(AsyncCommandGroup.ValidCommand)));
            commands.Should().Contain(ci => ci.MethodInfo == typeof(AsyncCommandGroup).GetMethod(nameof(AsyncCommandGroup.ValidCommand_2)));
        }

        private CommandsProvider CreateProvider(Type commandGroupType)
            => new CommandsProvider(
                commandsConfiguration: new CommandsConfiguration(new[] { commandGroupType }),
                commandInfoFactory: _commandInfoFactoryMock.Object);
    }
}