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

    private CommandMessageActionBuilder _subject = default!;
    
    [SetUp]
    public void Setup()
    {
        _serviceProviderMock = new Mock<IServiceProvider>();
        
        _commandGroupMock = new Mock<TestCommandGroup>();
        _serviceProviderMock
            .Setup(sp => sp.GetService(typeof(TestCommandGroup)))
            .Returns(_commandGroupMock.Object);
        
        _subject = new CommandMessageActionBuilder();
    }

    [Test]
    [TestCase(nameof(TestCommandGroup.ExecuteWithoutArgs))]
    [TestCase(nameof(TestCommandGroup.ExecuteWithoutArgsTask))]
    public async Task ShouldInvokeCommandGroupMethod(string methodName)
    {
        var commandGroupType = typeof(TestCommandGroup);
        var commandMethod = commandGroupType.GetMethod(methodName)!;

        var action = _subject.BuildInvokeCommand<IMessage, IUser<int>, int>(
            commandGroupType, commandMethod, typeof(SomeState));

        await action(_commandGroupMock.Object, null);

        if (methodName == nameof(TestCommandGroup.ExecuteWithoutArgs))
            _commandGroupMock.Verify(m => m.ExecuteWithoutArgs(), Times.Once);
        else if (methodName == nameof(TestCommandGroup.ExecuteWithoutArgsTask))
            _commandGroupMock.Verify(m => m.ExecuteWithoutArgsTask(), Times.Once);

        _commandGroupMock.VerifyNoOtherCalls();
    }
    
    [Test]
    [TestCase(nameof(TestCommandGroup.ExecuteWithStateArg))]
    [TestCase(nameof(TestCommandGroup.ExecuteWithStateArgTask))]
    public async Task ShouldInvokeCommandGroupMethodWithState(string methodName)
    {
        var commandGroupType = typeof(TestCommandGroup);
        var commandMethod = commandGroupType.GetMethod(methodName)!;

        var state = new SomeState();
        
        var action = _subject.BuildInvokeCommand<IMessage, IUser<int>, int>(
            commandGroupType, commandMethod, typeof(SomeState));

        await action(_commandGroupMock.Object, state);

        if (methodName == nameof(TestCommandGroup.ExecuteWithStateArg))
            _commandGroupMock.Verify(m => m.ExecuteWithStateArg(state), Times.Once);
        else if (methodName == nameof(TestCommandGroup.ExecuteWithStateArgTask))
            _commandGroupMock.Verify(m => m.ExecuteWithStateArgTask(state), Times.Once);

        _commandGroupMock.VerifyNoOtherCalls();
    }
    
    [Test]
    public async Task ShouldInvokeCommandGroupMethodWithStateAndExtraArg()
    {
        var commandGroupType = typeof(TestCommandGroup);
        var commandMethod = commandGroupType.GetMethod(nameof(TestCommandGroup.ExecuteWithStateArgAnExtraArg))!;

        var state = new SomeState();
        
        var action = _subject.BuildInvokeCommand<IMessage, IUser<int>, int>(
            commandGroupType, commandMethod, typeof(SomeState));

        await action(_commandGroupMock.Object, state);
        
        _commandGroupMock.Verify(m => m.ExecuteWithStateArgAnExtraArg(default!, state, default), Times.Once);
        _commandGroupMock.VerifyNoOtherCalls();
    }
    
    [Test]
    public async Task ShouldInvokeCommandGroupMethodWithStateAndExtraArgWhenStateIsNull()
    {
        var commandGroupType = typeof(TestCommandGroup);
        var commandMethod = commandGroupType.GetMethod(nameof(TestCommandGroup.ExecuteWithStateArgAnExtraArg))!;

        var action = _subject.BuildInvokeCommand<IMessage, IUser<int>, int>(
            commandGroupType, commandMethod, null);

        await action(_commandGroupMock.Object, null);
        
        _commandGroupMock.Verify(m => m.ExecuteWithStateArgAnExtraArg(default!, default!, default), Times.Once);
        _commandGroupMock.VerifyNoOtherCalls();
    }

    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class TestCommandGroup : CommandGroupBase<IMessage, IUser<int>, int>
    {
        public virtual void ExecuteWithoutArgs() { }

        public virtual Task ExecuteWithoutArgsTask() => Task.CompletedTask;

        public virtual void ExecuteWithStateArg(SomeState state) { }
        
        public virtual Task ExecuteWithStateArgTask(SomeState state) => Task.CompletedTask;

        public virtual void ExecuteWithStateArgAnExtraArg(string stringArg, SomeState state, int intArg) { }
    }

    public class SomeState : IState
    {
    }
}