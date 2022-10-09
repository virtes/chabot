using Chabot.Command;
using Chabot.Command.Configuration;
using Chabot.Command.Implementation;
using Chabot.Message;
using Chabot.State;
using Chabot.User;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace Chabot.UnitTests.Command;

public class CommandDescriptorsProviderTests
{
    private CommandsOptions _options = default!;
    
    private CommandDescriptorsProvider _subject = default!;

    [SetUp]
    public void Setup()
    {
        _options = new CommandsOptions();
        _options.AssembliesToScan.Add(typeof(CommandDescriptorsProviderTests).Assembly);
        
        var optionsMock = new Mock<IOptions<CommandsOptions>>();
        optionsMock
            .Setup(m => m.Value)
            .Returns(_options);

        _subject = new CommandDescriptorsProvider(optionsMock.Object);
    }

    [Test]
    public void ShouldParseDescriptors()
    {
        var actualDescriptors = _subject.GetCommandDescriptors();
        
        actualDescriptors.Should().BeEquivalentTo(new List<CommandDescriptor>
        {
            new CommandDescriptor
            {
                Type = typeof(CommandGroup),
                Method = typeof(CommandGroup).GetMethod(nameof(CommandGroup.Action))!,
                AllowedWithAnyCommandText = false,
                CommandTexts = new []{ CommandA },
                AllowedInAnyState = true,
                StateTypes = new []{ typeof(StateB) }
            },
            new CommandDescriptor
            {
                Type = typeof(CommandGroup),
                Method = typeof(CommandGroup).GetMethod(nameof(CommandGroup.ActionWithStateArg))!,
                AllowedWithAnyCommandText = true,
                CommandTexts = Array.Empty<string>(),
                AllowedInAnyState = true,
                StateTypes = new []{ typeof(StateA) }
            },
            new CommandDescriptor
            {
                Type = typeof(CommandGroup),
                Method = typeof(CommandGroup).GetMethod(nameof(CommandGroup.ActionWithCombinedState))!,
                AllowedWithAnyCommandText = true,
                CommandTexts = Array.Empty<string>(),
                AllowedInAnyState = false,
                StateTypes = new []{ typeof(StateA), typeof(StateB) }
            },
        });
    }

    public class CommandGroup : CommandGroupBase<IMessage, IUser<int>, int>
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

    public class StateA : IState { }
    public class StateB : IState { }

    private const string CommandA = "a";
}