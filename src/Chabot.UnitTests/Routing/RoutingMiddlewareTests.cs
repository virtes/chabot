using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chabot.Commands;
using Chabot.Processing;
using Chabot.Routing;
using Chabot.Routing.Implementation;
using Chabot.UnitTests.Fakes;
using Moq;
using NUnit.Framework;

namespace Chabot.UnitTests.Routing
{
    public class RoutingMiddlewareTests
    {
        private Mock<IMessageRouter<Message>> _messageRouterMock;

        [SetUp]
        public void Setup()
        {
            _messageRouterMock = new Mock<IMessageRouter<Message>>();
        }

        [Test]
        public async Task ShouldSetCommandToContextFromMessageRouter()
        {
            var message = new Message();
            var messageContextMock = new Mock<IMessageContext<Message>>();
            messageContextMock
                .Setup(c => c.Message)
                .Returns(message);

            var commandInfo = new CommandInfo(default!, default, default!);
            _messageRouterMock
                .Setup(r => r.RouteMessage(message))
                .Returns(commandInfo);

            var middleware = CreateMiddleware();

            await middleware.ExecuteAsync(messageContextMock.Object, _ => ValueTask.CompletedTask);

            messageContextMock.VerifySet(c => c.Command = commandInfo, Times.Once);
        }

        private RoutingMiddleware<Message> CreateMiddleware()
            => new RoutingMiddleware<Message>(_messageRouterMock.Object);
    }
}