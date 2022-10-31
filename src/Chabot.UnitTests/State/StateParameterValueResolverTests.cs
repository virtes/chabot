using Chabot.Message;
using Chabot.State;
using Chabot.State.Implementation;
using FluentAssertions;
using NUnit.Framework;

namespace Chabot.UnitTests.State;

public class StateParameterValueResolverTests
{
    private StateParameterValueResolver<Message, User> _subject = default!;

    [SetUp]
    public void Setup()
    {
        _subject = new StateParameterValueResolver<Message, User>();
    }

    [Test]
    public void ShouldReturnNullWhenParameterTypeIsNotCompatibleWithStateType()
    {
        var stateA = new StateA();
        var messageContext = CreateContext(stateA);

        var stateBParameter = typeof(SomeCommandGroup)
            .GetMethod(nameof(SomeCommandGroup.SomeCommand))!
            .GetParameters()
            .Single(p => p.ParameterType == typeof(StateB));

        var result = _subject.ResolveParameterValue(stateBParameter, messageContext);
        result.Should().BeNull();
    }

    [Test]
    public void ShouldReturnStateWhenParameterTypeIsEqualsToStateType()
    {
        var stateA = new StateA();
        var messageContext = CreateContext(stateA);

        var stateAParameter = typeof(SomeCommandGroup)
            .GetMethod(nameof(SomeCommandGroup.SomeCommand))!
            .GetParameters()
            .Single(p => p.ParameterType == typeof(StateA));

        var result = _subject.ResolveParameterValue(stateAParameter, messageContext);
        result.Should().BeSameAs(stateA);
    }

    private class SomeCommandGroup
    {
        public void SomeCommand(StateA stateA, StateB stateB) { }
    }

    // ReSharper disable once ClassNeverInstantiated.Local
    private class StateA : IState { }

    // ReSharper disable once ClassNeverInstantiated.Local
    private class StateB : IState { }

    private static MessageContext<Message, User> CreateContext(IState state)
        => new MessageContext<Message, User>(null!, null!, null!)
        {
            UserState = new UserState(state, DateTime.UtcNow, new Dictionary<string, string?>())
        };
}