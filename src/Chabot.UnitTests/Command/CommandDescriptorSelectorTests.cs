using Chabot.Command;
using Chabot.Command.Implementation;
using Chabot.State;
using FluentAssertions;
using Moq;
using NUnit.Framework;
// ReSharper disable InconsistentNaming

namespace Chabot.UnitTests.Command;

public class CommandDescriptorSelectorTests
{
    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void ShouldPreferCommandWithNotInAnyCommandTextWhenFoundByCommandText(bool reverse)
    {
        var commandA_AnyCommand_AnyState = new CommandDescriptor
        {
            AllowedWithAnyCommandText = true,
            CommandTexts = new []{ CommandA },
            AllowedInAnyState = true
        };
        var commandA_AnyState = new CommandDescriptor
        {
            AllowedWithAnyCommandText = false,
            CommandTexts = new []{ CommandA },
            AllowedInAnyState = true
        };
        
        var subject = CreateSelector(reverse, commandA_AnyState, commandA_AnyCommand_AnyState);
        
        subject.GetCommandDescriptor(CommandA, typeof(NotMappedState)).Should().BeSameAs(commandA_AnyState);
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void ShouldPreferCommandWithNotInAnyStateWhenFoundByState(bool reverse)
    {
        var anyCommand_stateA_AnyState = new CommandDescriptor
        {
            AllowedWithAnyCommandText = true,
            StateTypes = new []{ typeof(StateA) },
            AllowedInAnyState = true
        };
        
        var anyCommand_stateA = new CommandDescriptor
        {
            AllowedWithAnyCommandText = true,
            StateTypes = new []{ typeof(StateA) },
            AllowedInAnyState = false
        };
        
        var subject = CreateSelector(reverse, anyCommand_stateA_AnyState, anyCommand_stateA);

        subject.GetCommandDescriptor(NotMappedCommand, typeof(StateA)).Should().BeSameAs(anyCommand_stateA);
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void SpecifiedCommandTextAndStateType(bool reverse)
    {
        var commandA_stateA = new CommandDescriptor
        {
            CommandTexts = new []{ CommandA },
            StateTypes = new []{ typeof(StateA) }
        };
        
        var commandA_stateB = new CommandDescriptor
        {
            CommandTexts = new []{ CommandA },
            StateTypes = new []{ typeof(StateB) }
        };
        
        var commandB_stateA = new CommandDescriptor
        {
            CommandTexts = new []{ CommandB },
            StateTypes = new []{ typeof(StateA) }
        };
        
        var commandB_stateB = new CommandDescriptor
        {
            CommandTexts = new []{ CommandB },
            StateTypes = new []{ typeof(StateB) }
        };
        
        var subject = CreateSelector(reverse, commandA_stateA, commandA_stateB, commandB_stateA, commandB_stateB);

        subject.GetCommandDescriptor(CommandA, typeof(StateB)).Should().BeSameAs(commandA_stateB);
    }
    
    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void ShouldPreferMoreSpecificCommandText(bool reverse)
    {
        var commandAB_stateA = new CommandDescriptor
        {
            CommandTexts = new []{ CommandA, CommandB },
            StateTypes = new []{ typeof(StateA) }
        };
        
        var commandA_stateA = new CommandDescriptor
        {
            CommandTexts = new []{ CommandA },
            StateTypes = new []{ typeof(StateA) }
        };
        
        var subject = CreateSelector(reverse, commandAB_stateA, commandA_stateA);

        subject.GetCommandDescriptor(CommandA, typeof(StateA)).Should().BeSameAs(commandA_stateA);
    }
    
    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void ShouldPreferMoreSpecificStateType(bool reverse)
    {
        var commandA_stateA = new CommandDescriptor
        {
            CommandTexts = new []{ CommandA },
            StateTypes = new []{ typeof(StateA) }
        };
        
        var commandA_stateAB = new CommandDescriptor
        {
            CommandTexts = new []{ CommandA },
            StateTypes = new []{ typeof(StateA), typeof(StateB) }
        };
        
        var subject = CreateSelector(reverse, commandA_stateA, commandA_stateAB);

        subject.GetCommandDescriptor(CommandA, typeof(StateA)).Should().BeSameAs(commandA_stateA);
    }
    
    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void ShouldPreferMoreSpecificStateTypeWhenCommandTextNotFound(bool reverse)
    {
        var commandA_AnyCommand_stateA_AnyState = new CommandDescriptor
        {
            AllowedWithAnyCommandText = true,
            CommandTexts = new []{ CommandA },
            AllowedInAnyState = true,
            StateTypes = new []{ typeof(StateA) }
        };
        
        var commandA_AnyCommand_stateAB = new CommandDescriptor
        {
            AllowedWithAnyCommandText = true,
            CommandTexts = new []{ CommandA },
            AllowedInAnyState = false,
            StateTypes = new []{ typeof(StateA), typeof(StateB) }
        };
        
        var commandA_AnyCommand_stateAB_AnyState = new CommandDescriptor
        {
            AllowedWithAnyCommandText = true,
            CommandTexts = new []{ CommandA },
            AllowedInAnyState = true,
            StateTypes = new []{ typeof(StateA), typeof(StateB) }
        };
        
        var commandA_AnyCommand_stateABC_AnyState = new CommandDescriptor
        {
            AllowedWithAnyCommandText = true,
            CommandTexts = new []{ CommandA },
            AllowedInAnyState = true,
            StateTypes = new []{ typeof(StateA), typeof(StateB), typeof(StateC) }
        };
        
        var subject = CreateSelector(reverse, 
            commandA_AnyCommand_stateA_AnyState, commandA_AnyCommand_stateAB,
            commandA_AnyCommand_stateAB_AnyState, commandA_AnyCommand_stateABC_AnyState);

        subject.GetCommandDescriptor(NotMappedCommand, typeof(StateA)).Should().BeSameAs(commandA_AnyCommand_stateAB);
    }
    
    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void ShouldPreferMoreSpecificCommandTextTypeWhenCommandTextNotFound(bool reverse)
    {
        var commandA_AnyCommand_stateA_AnyState = new CommandDescriptor
        {
            AllowedWithAnyCommandText = true,
            CommandTexts = new []{ CommandA },
            AllowedInAnyState = true,
            StateTypes = new []{ typeof(StateA) }
        };
        
        var commandAB_stateA = new CommandDescriptor
        {
            AllowedWithAnyCommandText = false,
            CommandTexts = new []{ CommandA, CommandB },
            AllowedInAnyState = true,
            StateTypes = new []{ typeof(StateA) }
        };
        
        var commandAB_stateA_AnyState = new CommandDescriptor
        {
            AllowedWithAnyCommandText = true,
            CommandTexts = new []{ CommandA, CommandB },
            AllowedInAnyState = true,
            StateTypes = new []{ typeof(StateA) }
        };
        
        var commandABC_AnyCommand_stateA_AnyState = new CommandDescriptor
        {
            AllowedWithAnyCommandText = true,
            CommandTexts = new []{ CommandA, CommandB, CommandC },
            AllowedInAnyState = true,
            StateTypes = new []{ typeof(StateA) }
        };
        
        var subject = CreateSelector(reverse, 
            commandA_AnyCommand_stateA_AnyState, commandAB_stateA,
            commandAB_stateA_AnyState, commandABC_AnyCommand_stateA_AnyState);

        subject.GetCommandDescriptor(NotMappedCommand, typeof(StateA)).Should().BeSameAs(commandA_AnyCommand_stateA_AnyState);
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void ShouldPreferMostSpecificWhenCommandAndState(bool reverse)
    {
        var commandA_AnyCommand_stateA_AnyState = new CommandDescriptor
        {
            AllowedWithAnyCommandText = true,
            CommandTexts = new []{ CommandA },
            AllowedInAnyState = true,
            StateTypes = new []{ typeof(StateA) }
        };
        
        var commandA_AnyCommand_stateAB_AnyState = new CommandDescriptor
        {
            AllowedWithAnyCommandText = true,
            CommandTexts = new []{ CommandA },
            AllowedInAnyState = true,
            StateTypes = new []{ typeof(StateA), typeof(StateB) }
        };
        
        var subject = CreateSelector(reverse, 
            commandA_AnyCommand_stateA_AnyState, commandA_AnyCommand_stateAB_AnyState);
        
        subject.GetCommandDescriptor(NotMappedCommand, typeof(NotMappedState))
            .Should().BeSameAs(commandA_AnyCommand_stateA_AnyState);
    }
    
    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void ShouldPreferMostSpecificWhenCommandAndState_2(bool reverse)
    {
        var commandA_AnyCommand_stateA_AnyState = new CommandDescriptor
        {
            AllowedWithAnyCommandText = true,
            CommandTexts = new []{ CommandA },
            AllowedInAnyState = true,
            StateTypes = new []{ typeof(StateA) }
        };
        
        var commandAB_AnyCommand_stateA_AnyState = new CommandDescriptor
        {
            AllowedWithAnyCommandText = true,
            CommandTexts = new []{ CommandA, CommandB },
            AllowedInAnyState = true,
            StateTypes = new []{ typeof(StateA) }
        };
        
        var subject = CreateSelector(reverse, 
            commandA_AnyCommand_stateA_AnyState, commandAB_AnyCommand_stateA_AnyState);
        
        subject.GetCommandDescriptor(NotMappedCommand, typeof(NotMappedState))
            .Should().BeSameAs(commandA_AnyCommand_stateA_AnyState);
    }
    
    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void ShouldPreferMostSpecificWhenCommandAndState_3(bool reverse)
    {
        var commandA_AnyCommand_stateA_AnyState = new CommandDescriptor
        {
            AllowedWithAnyCommandText = true,
            CommandTexts = new []{ CommandA },
            AllowedInAnyState = true,
            StateTypes = new []{ typeof(StateA) }
        };
        
        var anyCommand_stateA_AnyState = new CommandDescriptor
        {
            AllowedWithAnyCommandText = true,
            CommandTexts = new string[]{ },
            AllowedInAnyState = true,
            StateTypes = new []{ typeof(StateA) }
        };
        
        var subject = CreateSelector(reverse, 
            commandA_AnyCommand_stateA_AnyState, anyCommand_stateA_AnyState);
        
        subject.GetCommandDescriptor(NotMappedCommand, typeof(NotMappedState))
            .Should().BeSameAs(anyCommand_stateA_AnyState);
    }
    
    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void ShouldPreferMostSpecificWhenCommandAndState_4(bool reverse)
    {
        var commandA_AnyCommand_stateA_AnyState = new CommandDescriptor
        {
            AllowedWithAnyCommandText = true,
            CommandTexts = new []{ CommandA },
            AllowedInAnyState = true,
            StateTypes = new []{ typeof(StateA) }
        };
        
        var commandA_AnyCommand_AnyState = new CommandDescriptor
        {
            AllowedWithAnyCommandText = true,
            CommandTexts = new []{ CommandA },
            AllowedInAnyState = true,
            StateTypes = new Type[]{ }
        };
        
        var subject = CreateSelector(reverse, 
            commandA_AnyCommand_stateA_AnyState, commandA_AnyCommand_AnyState);
        
        subject.GetCommandDescriptor(NotMappedCommand, typeof(NotMappedState))
            .Should().BeSameAs(commandA_AnyCommand_AnyState);
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void ShouldValidateCommandTextWhenMatchOnlyByState(bool reverse)
    {
        var anyCommandText_OnlyStateA = new CommandDescriptor
        {
            AllowedWithAnyCommandText = true,
            CommandTexts = Array.Empty<string>(),
            AllowedInAnyState = false,
            StateTypes = new []{ typeof(StateA) }
        };

        var onlyCommandA_OnlyStateA = new CommandDescriptor
        {
            AllowedWithAnyCommandText = false,
            CommandTexts = new []{ CommandA },
            AllowedInAnyState = false,
            StateTypes = new []{ typeof(StateA) }
        };

        var subject = CreateSelector(reverse,
            anyCommandText_OnlyStateA, onlyCommandA_OnlyStateA);

        subject.GetCommandDescriptor(NotMappedCommand, typeof(StateA))
            .Should().BeSameAs(anyCommandText_OnlyStateA);
    }

    private static CommandDescriptorSelector CreateSelector(bool reverse, params CommandDescriptor[] descriptors)
    {
        var commandDescriptorsProviderMock = new Mock<ICommandDescriptorsProvider>();
        commandDescriptorsProviderMock
            .Setup(m => m.GetCommandDescriptors())
            .Returns(reverse 
                ? descriptors.Reverse().ToArray() 
                : descriptors);
        
        return new CommandDescriptorSelector(commandDescriptorsProviderMock.Object);
    }
    
    private const string CommandA = "A";
    private const string CommandB = "B";
    private const string CommandC = "C";
    private const string NotMappedCommand = "not-a-command";
    
    private class StateA : IState { }
    private class StateB : IState { }
    private class StateC : IState { }
    private class NotMappedState : IState { }
}