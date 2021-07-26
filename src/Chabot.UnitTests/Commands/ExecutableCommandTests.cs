using System.Threading.Tasks;
using Chabot.Commands;
using Chabot.Commands.Implementation;
using Chabot.Processing;
using Chabot.UnitTests.Fakes;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Chabot.UnitTests.Commands
{
    public class ExecutableCommandTests
    {
        [Test]
        public async Task ShouldExecutePassedFunc()
        {
            var contextMock = new Mock<IMessageContext<Message>>();

            var commandInfo = new CommandInfo(default!, default, default!);

            var funcCalledTimes = 0;

            var executableCommand = new ExecutableCommand<Message>(commandInfo, async c =>
            {
                await Task.Delay(0);

                c.Should().BeSameAs(contextMock.Object);

                ++funcCalledTimes;
            });

            await executableCommand.ExecuteAsync(contextMock.Object);

            funcCalledTimes.Should().Be(1);
        }

        [Test]
        public void ShouldReturnPassedCommandInfo()
        {
            var commandInfo = new CommandInfo(default!, default, default!);

            var executableCommand = new ExecutableCommand<Message>(commandInfo, _ => ValueTask.CompletedTask);

            executableCommand.CommandInfo.Should().BeSameAs(commandInfo);
        }
    }
}