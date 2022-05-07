using Chabot.State.Configuration;
using Chabot.State.Exceptions;
using Chabot.State.Implementation;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace Chabot.UnitTests.State;

public class StateTypeMappingTests
{
    private StateTypeMappingOptions _options = default!;

    private StateTypeMapping _subject = default!;
    
    [SetUp]
    public void Setup()
    {
        _options = new StateTypeMappingOptions();

        var optionsMock = new Mock<IOptions<StateTypeMappingOptions>>();
        optionsMock
            .Setup(o => o.Value)
            .Returns(_options);

        _subject = new StateTypeMapping(optionsMock.Object);
    }

    [Test]
    public void ShouldProvideStateTypeKey()
    {
        var actualTypeName = _subject.GetStateTypeKey(typeof(SomeTestState));

        actualTypeName.Should().Be("Chabot.UnitTests.State.StateTypeMappingTests+SomeTestState");
    }

    [Test]
    public void ShouldProvideStateTypeFromAssemblyByKey()
    {
        _options.AssembliesToScan.Add(typeof(StateTypeMappingTests).Assembly);

        var actualType = _subject.GetStateType("Chabot.UnitTests.State.StateTypeMappingTests+SomeTestState");

        actualType.Should().Be(typeof(SomeTestState));
    }

    [Test]
    public void ShouldThrowWhenStateTypeNotFound()
    {
        var getStateType = () => _subject.GetStateType("Chabot.UnitTests.State.StateTypeMappingTests+SomeTestState");

        getStateType.Should().Throw<StateTypeNotFoundException>();
    }

    private class SomeTestState
    {
    }
}