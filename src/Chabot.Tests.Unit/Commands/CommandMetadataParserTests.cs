using System.Reflection;
using FluentAssertions;
using Chabot.Commands;
using Moq;
using Xunit;

namespace Chabot.Tests.Unit.Commands;

public class CommandMetadataParserTests
{
    [Fact]
    public void ParseCommands_ShouldParseCommands()
    {
        var commandAMethod = GetMethodInfo<Commands>(nameof(Commands.CommandA));
        var commandBMethod = GetMethodInfo<Commands>(nameof(Commands.CommandB));

        var commandBRestrictions = new List<object> { new() };

        var restrictionsFactoryMock = new Mock<IRestrictionsFactory>();
        restrictionsFactoryMock
            .Setup(rf => rf.CreateRestrictions(typeof(Commands), commandAMethod))
            .Returns(Array.Empty<object>());
        restrictionsFactoryMock
            .Setup(rf => rf.CreateRestrictions(typeof(Commands), commandBMethod))
            .Returns(commandBRestrictions);

        var subject = CreateSubject(restrictionsFactoryMock.Object);

        var commands = subject.ParseCommands(typeof(Commands));
        commands.Should().BeEquivalentTo(new []
        {
            new CommandMetadata
            {
                Id = "commanda(arg1,arg2)",
                Type = typeof(Commands),
                MethodInfo = commandAMethod,
                Restrictions = Array.Empty<object>(),
                Order = 0
            },
            new CommandMetadata
            {
                Id = "commandb()",
                Type = typeof(Commands),
                MethodInfo = commandBMethod,
                Restrictions = commandBRestrictions,
                Order = 2
            }
        });
    }

    [Fact]
    public void ParseCommands_ShouldNotParsePrivateMethods()
    {
        var subject = CreateSubject();

        var actualCommands = subject.ParseCommands(typeof(PrivateCommands));

        actualCommands.Should().BeEmpty();
    }

    // ReSharper disable once MemberCanBePrivate.Global
    internal class Commands
    {
#pragma warning disable CA1822
        public void CommandA(string arg1, int arg2)
#pragma warning restore CA1822
        {
        }

        [Order(2)]
#pragma warning disable CA1822
        public Task CommandB()
#pragma warning restore CA1822
        {
            return Task.CompletedTask;
        }
    }

    // ReSharper disable once MemberCanBePrivate.Global
    internal class PrivateCommands
    {
        [Order(1)]
#pragma warning disable CA1822
        // ReSharper disable once UnusedMember.Local
        private void SomeCommand()
#pragma warning restore CA1822
        {
        }
    }

    private static MethodInfo GetMethodInfo<T>(string methodName)
    {
        return typeof(T).GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public)!;
    }

    private static CommandMetadataParser CreateSubject(params IRestrictionsFactory[] restrictionsFactories)
        => new(restrictionsFactories);
}