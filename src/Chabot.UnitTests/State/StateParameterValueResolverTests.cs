using Chabot.Message;
using Chabot.State;
using Chabot.State.Implementation;
using Chabot.User;
using FluentAssertions;
using NUnit.Framework;

namespace Chabot.UnitTests.State;

public class StateParameterValueResolverTests
{
    private StateParameterValueResolver<IMessage, IUser<int>, int> _subject = default!;

    [SetUp]
    public void Setup()
    {
        _subject = new StateParameterValueResolver<IMessage, IUser<int>, int>();
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

    private static MessageContext<IMessage, IUser<int>, int> CreateContext(IState state)
        => new MessageContext<IMessage, IUser<int>, int>(null!, null!, null!)
        {
            UserState = new UserState(state, DateTime.UtcNow, new Dictionary<string, string?>())
        };
}