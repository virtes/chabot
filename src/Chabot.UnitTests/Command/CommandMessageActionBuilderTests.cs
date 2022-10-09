using Chabot.Command;
using Chabot.Command.Implementation;
using Chabot.Message;
using Chabot.State;
using Chabot.User;
using Moq;
using NUnit.Framework;

namespace Chabot.UnitTests.Command;

public class CommandMessageActionBuilderTests
{
    private Mock<IServiceProvider> _serviceProviderMock = default!;
    private Mock<TestCommandGroup> _commandGroupMock = default!;
    
    [SetUp]
    public void Setup()
    {
        _serviceProviderMock = new Mock<IServiceProvider>();
        
        _commandGroupMock = new Mock<TestCommandGroup>();
        _serviceProviderMock
            .Setup(sp => sp.GetService(typeof(TestCommandGroup)))
            .Returns(_commandGroupMock.Object);
    }

    [Test]
    [TestCase(nameof(TestCommandGroup.Execute))]
    [TestCase(nameof(TestCommandGroup.ExecuteTask))]
    public async Task ShouldInvokeParameterlessCommandGroupMethodWithoutValueResolver(string methodName)
    {
        var commandGroupType = typeof(TestCommandGroup);
        var commandMethod = commandGroupType.GetMethod(methodName)!;

        var subject = CreateSubject();
        var action = subject.BuildInvokeCommand(commandGroupType, commandMethod);

        var messageContext = CreateContext(DefaultState.Instance);
        await action(_commandGroupMock.Object, messageContext);

        if (methodName == nameof(TestCommandGroup.Execute))
            _commandGroupMock.Verify(m => m.Execute(), Times.Once);
        else if (methodName == nameof(TestCommandGroup.ExecuteTask))
            _commandGroupMock.Verify(m => m.ExecuteTask(), Times.Once);

        _commandGroupMock.VerifyNoOtherCalls();
    }

    [Test]
    [TestCase(nameof(TestCommandGroup.ExecuteWithParameters))]
    [TestCase(nameof(TestCommandGroup.ExecuteWithParametersTask))]
    public async Task ShouldInvokeCommandGroupMethodWithoutValueResolver(string methodName)
    {
        var commandGroupType = typeof(TestCommandGroup);
        var commandMethod = commandGroupType.GetMethod(methodName)!;

        var subject = CreateSubject();
        var action = subject.BuildInvokeCommand(commandGroupType, commandMethod);

        var messageContext = CreateContext(DefaultState.Instance);
        await action(_commandGroupMock.Object, messageContext);

        if (methodName == nameof(TestCommandGroup.ExecuteWithParameters))
            _commandGroupMock.Verify(m => m.ExecuteWithParameters(null, null), Times.Once);
        else if (methodName == nameof(TestCommandGroup.ExecuteWithParametersTask))
            _commandGroupMock.Verify(m => m.ExecuteWithParametersTask(null, null), Times.Once);

        _commandGroupMock.VerifyNoOtherCalls();
    }

    [Test]
    [TestCase(nameof(TestCommandGroup.ExecuteWithParameters), null)]
    [TestCase(nameof(TestCommandGroup.ExecuteWithParametersTask), null)]
    [TestCase(nameof(TestCommandGroup.ExecuteWithParameters), "some_value")]
    [TestCase(nameof(TestCommandGroup.ExecuteWithParametersTask), "some_value")]
    public async Task ShouldInvokeCommandGroupMethodWithValueResolver(string methodName, string? parameterValue)
    {
        var commandGroupType = typeof(TestCommandGroup);
        var commandMethod = commandGroupType.GetMethod(methodName)!;

        var messageContext = CreateContext(DefaultState.Instance);
        var parameterInfo = commandMethod.GetParameters().Single(p => p.Name == "parameter");

        var valueResolverMock = new Mock<ICommandParameterValueResolver<IMessage, IUser<int>, int>>();
        valueResolverMock
            .Setup(vr => vr.ResolveParameterValue(parameterInfo, messageContext))
            .Returns(parameterValue);

        var valueResolverMockFactory = new Mock<ICommandParameterValueResolverFactory<IMessage, IUser<int>, int>>();
        valueResolverMockFactory
            .Setup(vrf => vrf.CreateValueResolver(parameterInfo))
            .Returns(valueResolverMock.Object);

        var subject = CreateSubject(valueResolverMockFactory.Object);
        var action = subject.BuildInvokeCommand(commandGroupType, commandMethod);

        await action(_commandGroupMock.Object, messageContext);

        if (methodName == nameof(TestCommandGroup.ExecuteWithParameters))
            _commandGroupMock.Verify(m => m.ExecuteWithParameters(parameterValue, null), Times.Once);
        else if (methodName == nameof(TestCommandGroup.ExecuteWithParametersTask))
            _commandGroupMock.Verify(m => m.ExecuteWithParametersTask(parameterValue, null), Times.Once);

        _commandGroupMock.VerifyNoOtherCalls();
    }

    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class TestCommandGroup : CommandGroupBase<IMessage, IUser<int>, int>
    {
        public virtual void Execute() { }

        public virtual Task ExecuteTask() => Task.CompletedTask;

        public virtual void ExecuteWithParameters(string? parameter, string? extraParameter) { }
        
        public virtual Task ExecuteWithParametersTask(string? parameter, string? extraParameter)
            => Task.CompletedTask;
    }

    private static MessageContext<IMessage, IUser<int>, int> CreateContext(IState state)
    {
        return new MessageContext<IMessage, IUser<int>, int>(
            services: default!,
            message: default!,
            user: default!)
        {
            UserState = new UserState(state, DateTime.UtcNow, new Dictionary<string, string?>())
        };
    }

    private static CommandMessageActionBuilder<IMessage, IUser<int>, int> CreateSubject(
        params ICommandParameterValueResolverFactory<IMessage, IUser<int>, int>[] resolverFactories)
    {
        return new CommandMessageActionBuilder<IMessage, IUser<int>, int>(resolverFactories);
    }
}