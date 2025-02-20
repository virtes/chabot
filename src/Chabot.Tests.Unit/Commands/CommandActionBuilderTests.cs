using System.Reflection;
using FluentAssertions;
using Chabot.Commands;
using Moq;
using Xunit;

namespace Chabot.Tests.Unit.Commands;

public class CommandActionBuilderTests
{
    private readonly Mock<IServiceProvider> _serviceProviderMock = new(MockBehavior.Strict);
    private readonly Mock<Commands> _commandsMock = new(MockBehavior.Strict);
    private const string Update = "update";

    [Fact]
    public async Task BuildCommandAction_MethodWithoutArgs_void_ShouldBuildAction()
    {
        var subject = CreateSubject();

        var commandAction = subject.BuildCommandAction(typeof(Commands), GetMethodInfo(nameof(Commands.VoidParameterless)));

        var updateContext = CreateUpdateContext();
        _commandsMock.Setup(c => c.VoidParameterless());

        await commandAction(_commandsMock.Object, updateContext);

        _commandsMock.VerifyAll();
    }

    [Fact]
    public async Task BuildCommandAction_MethodWithoutArgs_Task_ShouldBuildAction()
    {
        var subject = CreateSubject();

        var commandAction = subject.BuildCommandAction(typeof(Commands), GetMethodInfo(nameof(Commands.TaskParameterless)));

        var resultTask = Task.CompletedTask;

        var updateContext = CreateUpdateContext();
        _commandsMock.Setup(c => c.TaskParameterless()).Returns(resultTask);

        var actualResult = commandAction(_commandsMock.Object, updateContext);
        actualResult.Should().Be(resultTask);

        await resultTask;

        _commandsMock.VerifyAll();
    }

    [Fact]
    public async Task BuildCommandAction_ParameterWithoutResolver_ShouldBuildActionWithDefaultArgValue()
    {
        var updateContext = CreateUpdateContext();
        var methodInfo = GetMethodInfo(nameof(Commands.VoidStringParameter));
        var parameterInfo = methodInfo.GetParameters().Single();

        var valueResolverFactoryMock = new Mock<ICommandParameterValueResolverFactory<string>>(MockBehavior.Strict);
        var valueResolverOutParameter = It.IsAny<ICommandParameterValueResolver<string>>();
        valueResolverFactoryMock
            .Setup(vrf => vrf.TryCreate(parameterInfo, out valueResolverOutParameter))
            .Callback((ParameterInfo _, out ICommandParameterValueResolver<string> resolver) =>
            {
                resolver = null!;
            })
            .Returns(false);

        var subject = CreateSubject(valueResolverFactoryMock.Object);

        var commandAction = subject.BuildCommandAction(typeof(Commands), GetMethodInfo(nameof(Commands.VoidStringParameter)));

        _commandsMock.Setup(c => c.VoidStringParameter(null!));

        await commandAction(_commandsMock.Object, updateContext);

        _commandsMock.VerifyAll();
        valueResolverFactoryMock.VerifyAll();
    }

    [Fact]
    public async Task BuildCommandAction_ParameterWithResolver_ShouldBuildActionWithValueFromResolver()
    {
        var updateContext = CreateUpdateContext();
        var methodInfo = GetMethodInfo(nameof(Commands.VoidStringParameter));
        var parameterInfo = methodInfo.GetParameters().Single();

        var parameterValue = Guid.NewGuid().ToString();

        var valueResolverMock = new Mock<ICommandParameterValueResolver<string>>(MockBehavior.Strict);
        valueResolverMock
            .Setup(pvr => pvr.ResolveParameterValue(parameterInfo, updateContext))
            .Returns(() => ValueTask.FromResult<object?>(parameterValue));

        var valueResolverFactoryMock = new Mock<ICommandParameterValueResolverFactory<string>>(MockBehavior.Strict);
        var valueResolverOutParameter = It.IsAny<ICommandParameterValueResolver<string>>();
        valueResolverFactoryMock
            .Setup(vrf => vrf.TryCreate(parameterInfo, out valueResolverOutParameter))
            .Callback((ParameterInfo _, out ICommandParameterValueResolver<string> resolver) =>
            {
                resolver = valueResolverMock.Object;
            })
            .Returns(true);

        var subject = CreateSubject(valueResolverFactoryMock.Object);

        var commandAction = subject.BuildCommandAction(typeof(Commands), methodInfo);

        _commandsMock.Setup(c => c.VoidStringParameter(parameterValue));

        await commandAction(_commandsMock.Object, updateContext);

        _commandsMock.VerifyAll();
    }

    internal class Commands : CommandsBase<string>
    {
        public virtual void VoidParameterless()
        {
        }

        public virtual Task TaskParameterless() => Task.CompletedTask;

        public virtual void VoidStringParameter(string arg1)
        {
        }
    }

    private static MethodInfo GetMethodInfo(string methodName)
    {
        return typeof(Commands).GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public)!;
    }

    private UpdateContext<string> CreateUpdateContext()
    {
        return new UpdateContext<string>(_serviceProviderMock.Object, default!, Update);
    }

    private static CommandActionBuilder<string> CreateSubject(
        params ICommandParameterValueResolverFactory<string>[] resolverFactories)
    {
        return new CommandActionBuilder<string>(resolverFactories);
    }
}