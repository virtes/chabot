using System.Reflection;
using Chabot.Command;
using Chabot.Command.Implementation;
using Chabot.State;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Chabot.UnitTests.Command;

public class CommandDescriptorParserTests
{
    private Mock<IServiceProvider> _serviceProviderMock = default!;

    private CommandDescriptorParser _subject = default!;

    [SetUp]
    public void Setup()
    {
        _serviceProviderMock = new Mock<IServiceProvider>(MockBehavior.Strict);

        _subject = new CommandDescriptorParser(_serviceProviderMock.Object);
    }

    [Test]
    public void ParseCommandDescriptors_SimpleCommandAttributes_ShouldParseDescriptors()
    {
        var actualDescriptors = _subject.ParseCommandDescriptors(typeof(CommandGroupSimpleCommandAttributes));

        actualDescriptors.Should().BeEquivalentTo(new List<CommandDescriptor>
        {
            new CommandDescriptor
            {
                Type = typeof(CommandGroupSimpleCommandAttributes),
                Method = typeof(CommandGroupSimpleCommandAttributes).GetMethod(nameof(CommandGroupSimpleCommandAttributes.Action))!,
                AllowedWithAnyCommandText = false,
                CommandTexts = new []{ CommandA },
                AllowedInAnyState = true,
                StateTypes = new []{ typeof(StateB) }
            },
            new CommandDescriptor
            {
                Type = typeof(CommandGroupSimpleCommandAttributes),
                Method = typeof(CommandGroupSimpleCommandAttributes).GetMethod(nameof(CommandGroupSimpleCommandAttributes.ActionWithStateArg))!,
                AllowedWithAnyCommandText = true,
                CommandTexts = Array.Empty<string>(),
                AllowedInAnyState = true,
                StateTypes = new []{ typeof(StateA) }
            },
            new CommandDescriptor
            {
                Type = typeof(CommandGroupSimpleCommandAttributes),
                Method = typeof(CommandGroupSimpleCommandAttributes).GetMethod(nameof(CommandGroupSimpleCommandAttributes.ActionWithCombinedState))!,
                AllowedWithAnyCommandText = true,
                CommandTexts = Array.Empty<string>(),
                AllowedInAnyState = false,
                StateTypes = new []{ typeof(StateA), typeof(StateB) }
            },
        });
    }

    [Test]
    public void ParseCommandDescriptors_GenericCommandAttributesFromServiceProvider_ShouldParseDescriptors()
    {
        var commandTextProvider = new CommandGroupGenericCommandAttributeServiceProvider.CommandTextProvider(new[]
        {
            "command-1",
            "command-2"
        });

        _serviceProviderMock
            .Setup(sp => sp.GetService(typeof(CommandGroupGenericCommandAttributeServiceProvider.CommandTextProvider)))
            .Returns(commandTextProvider);

        var actualDescriptors = _subject.ParseCommandDescriptors(typeof(CommandGroupGenericCommandAttributeServiceProvider));
        actualDescriptors.Should().BeEquivalentTo(new List<CommandDescriptor>
        {
            new CommandDescriptor
            {
                Type = typeof(CommandGroupGenericCommandAttributeServiceProvider),
                Method = typeof(CommandGroupGenericCommandAttributeServiceProvider).GetMethod(nameof(CommandGroupGenericCommandAttributeServiceProvider.Action))!,
                AllowedWithAnyCommandText = false,
                CommandTexts = new []{ "command-1", "command-2" },
                AllowedInAnyState = true,
                StateTypes = Array.Empty<Type>()
            }
        });
    }

    [Test]
    public void ParseCommandDescriptors_GenericCommandAttributesUsingActivator_ShouldParseDescriptors()
    {
        _serviceProviderMock
            .Setup(sp => sp.GetService(typeof(CommandGroupGenericCommandAttribute.CommandTextProvider)))
            .Returns(null);

        var actualDescriptors = _subject.ParseCommandDescriptors(typeof(CommandGroupGenericCommandAttribute));
        actualDescriptors.Should().BeEquivalentTo(new List<CommandDescriptor>
        {
            new CommandDescriptor
            {
                Type = typeof(CommandGroupGenericCommandAttribute),
                Method = typeof(CommandGroupGenericCommandAttribute).GetMethod(nameof(CommandGroupGenericCommandAttribute.Action))!,
                AllowedWithAnyCommandText = false,
                CommandTexts = new []{ "command-1", "command-2" },
                AllowedInAnyState = true,
                StateTypes = Array.Empty<Type>()
            }
        });
    }

    public class CommandGroupSimpleCommandAttributes : CommandGroupBase<Message, User>
    {
        [Command(CommandA)]
        [AllowedInAnyState]
        [AllowedState(typeof(StateB))]
        public void Action() { }

        [Command(AllowedWithAnyCommandText = true)]
        [AllowedInAnyState]
        public void ActionWithStateArg(StateA state) { }

        [Command(AllowedWithAnyCommandText = true)]
        [AllowedState(typeof(StateB))]
        public void ActionWithCombinedState(StateA stateA, StateB stateB) { }
    }

    public class CommandGroupGenericCommandAttributeServiceProvider : CommandGroupBase<Message, User>
    {
        [Command<CommandTextProvider>]
        [AllowedInAnyState]
        public void Action() { }

        public class CommandTextProvider : ICommandTextProvider
        {
            private readonly string[] _commandTexts;

            public CommandTextProvider(string[] commandTexts)
            {
                _commandTexts = commandTexts;
            }

            public string[] GetCommandTexts(MethodInfo methodInfo)
            {
                methodInfo.Name.Should().Be(nameof(Action));

                return _commandTexts;
            }
        }
    }

    public class CommandGroupGenericCommandAttribute : CommandGroupBase<Message, User>
    {
        [Command<CommandTextProvider>]
        [AllowedInAnyState]
        public void Action() { }

        public class CommandTextProvider : ICommandTextProvider
        {
            public string[] GetCommandTexts(MethodInfo methodInfo)
            {
                methodInfo.Name.Should().Be(nameof(Action));

                return new[] { "command-1", "command-2" };
            }
        }
    }

    public class StateA : IState { }
    public class StateB : IState { }

    private const string CommandA = "a";
}